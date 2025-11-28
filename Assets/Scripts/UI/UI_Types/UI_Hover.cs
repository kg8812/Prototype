using Apis;
using Default;
using UnityEngine;

namespace UI
{
    public class UI_Hover : UI_Base
    {
        protected RectTransform _contentTrans;
        protected readonly Vector3 offsetVec = new(0, -1080, 0);

        private void Update()
        {
            if (_activated)
                SetPosition();
        }

        public override void Init()
        {
            base.Init();
            UIManager.SetCanvas(this, UIType.Hover);
            SetPosition();
        }

        private void SetPosition()
        {
            if (!ReferenceEquals(_contentTrans, null))
                _contentTrans.anchoredPosition = Input.mousePosition + offsetVec;
        }

        public override void TryActivated(bool force = false)
        {
            SetPosition();
            base.TryActivated(force);
        }
    }
}