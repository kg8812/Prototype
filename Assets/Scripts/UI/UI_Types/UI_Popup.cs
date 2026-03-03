using Apis;
using UnityEngine;

namespace Default
{

    public class UI_Popup : UI_Base, IController
    {
        public override void Init()
        {
            base.Init();
            UIManager.SetCanvas(this, UIType.Popup);
        }

        protected override void Activated()
        {
            base.Activated();
            GameManager.UI.RegisterUIController(this);
        }

        public virtual void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Cancel)))
            {
                CloseOwn();
            }
        }
        
        public virtual void GamePadControl()
        {
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Cancel)))
            {
                CloseOwn();
            }
        }

        private void OnDestroy()
        {
            if(_activated && !GameManager.IsQuitting)
                GameManager.UI.RemoveController(this);
        }
    }
}