using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_DragItem : UI_Hover
    {
        public Image DragImg;

        public override void Init()
        {
            base.Init();
            Bind<RectTransform>(typeof(Rects));
            _contentTrans = Get<RectTransform>((int)Rects.ItemImg);
        }

        private enum Rects
        {
            ItemImg
        }

        // public override void TryActivated(bool force = false)
        // {
        //     // GameManager.UI.RegisterUI(this);
        //     // DragImg.enabled = true;
        //     // _activated = true;
        //     // base.TryActivated(force);
        // }
        //
        // public override void TryDeactivated(bool force = false)
        // {
        //     // base.TryDeactivated(force);
        //     _activated = false;
        //     DragImg.enabled = false;
        // }
    }
}