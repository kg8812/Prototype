using Apis;

namespace Default
{
    public class UI_Popup : UI_Base, IController
    {
        private void OnDestroy()
        {
            if (_activated && !GameManager.IsQuitting)
                GameManager.UI.RemoveController(this);
        }

        public virtual void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Cancel))) CloseOwn();
        }

        public virtual void GamePadControl()
        {
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Cancel))) CloseOwn();
        }

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
    }
}