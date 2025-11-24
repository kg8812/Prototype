using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Apis.UI
{
    public class UIAsset_Button: UIEffector
    {
        public UnityEvent OnClick;

        public override void Init()
        {
            base.Init();
            OnClick ??= new UnityEvent();
        }

        public override void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                OnPointerClick(null);
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Select)))
            {
                OnPointerClick(null);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (!IsDisable)
            {
                OnClick.Invoke();
                PressOff();
            }
        }
    }
}