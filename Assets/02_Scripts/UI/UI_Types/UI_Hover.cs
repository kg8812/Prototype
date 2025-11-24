using Apis;
using Default;
using UnityEngine;

namespace UI
{
    public class UI_Hover: UI_Base
    {
        protected RectTransform _contentTrans = null;
        protected readonly Vector3 offsetVec = new Vector3(0, -1080, 0);
        
        public override void Init()
        {
            base.Init();
            UIManager.SetCanvas(this, UIType.Hover);
            SetPosition();   
        }
        
        private void Update()
        {
            if(_activated)
                SetPosition();
        }

        private void SetPosition()
        {
            if(!ReferenceEquals(_contentTrans, null))
                _contentTrans.anchoredPosition = Input.mousePosition + offsetVec;
        }

        public override void TryActivated(bool force = false)
        {
            SetPosition();
            base.TryActivated(force);
        }
    }
}