using Apis.UI.Focus;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UIAsset_Slider : UIEffector
    {
        public Slider slider;
        public NavigationMode mode;
        public float moveDir = 0.1f;

        private float curValue;

        public override void Init()
        {
            base.Init();
            curValue = slider.value;
        }

        public void MovePre()
        {
            curValue = Mathf.Max(0, curValue - moveDir);
            UpdateValue();
        }

        public void MoveNext()
        {
            curValue = Mathf.Min(1, curValue + moveDir);
            UpdateValue();
        }

        private void UpdateValue()
        {
            slider.value = curValue;
        }


        public override void KeyControl()
        {
            if (mode == NavigationMode.Horizontal)
            {
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left))) MovePre();
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right))) MoveNext();
            }
            else
            {
                // 반대로 위쪽이 증가하는 방향
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Up))) MoveNext();
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Down))) MovePre();
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();

            if (mode == NavigationMode.Horizontal)
            {
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Left))) MovePre();
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Right))) MoveNext();
            }
            else
            {
                // 반대로 위쪽이 증가하는 방향
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Up))) MoveNext();
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Down))) MovePre();
            }
        }
    }
}