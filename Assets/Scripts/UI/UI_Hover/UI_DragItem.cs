using System;
using UnityEngine;
using UnityEngine.UI;


namespace Apis.UI
{
    public class UI_DragItem: UI_Hover
    {
        public Image DragImg;
        private Guid _dragGuid;

        enum Rects
        {
            ItemImg
        }

        public override void Init()
        {
            base.Init();
            Bind<RectTransform>(typeof(Rects));
            _contentTrans = Get<RectTransform>((int)Rects.ItemImg);
            DragOff();
        }

        public void DragOn()
        {
            DragImg.enabled = true; 
            _dragGuid = GameManager.instance.PreventControlOn();
            SetPosition();
        }

        public void DragOff()
        {
            DragImg.enabled = false;
            GameManager.instance.PreventControlOff(_dragGuid);
            
        }
        public override void TryActivated(bool force = false)
        {
            base.TryActivated(force);
        }
        
        public override void TryDeactivated(bool force = false)
        {
            base.TryDeactivated(force);
            DragOff();
        }
    }
}