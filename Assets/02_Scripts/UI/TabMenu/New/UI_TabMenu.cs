using System;
using Apis.UI;
using Default;
using UI;
using UnityEngine;

namespace Apis
{
    public delegate void OnUIOpen();
    public class UI_TabMenu: UI_Scene
    {
        public static Action<bool> OnUiToggle;
        [SerializeField] private UI_DragItem dragImg;
        public UI_HeaderMenu_nton tabHeaderMenuNtoN;

        public override void Init()
        {
            base.Init();
            tabHeaderMenuNtoN.Init();
            dragImg.Init();
            foreach (var value in tabHeaderMenuNtoN.contentControllers)
            {
                AddSubItem(value);
            }
        }

        public void MoveHeader(int index)
        {
            tabHeaderMenuNtoN.headerController.MoveTo(index);
        }

        public override void TryActivated(bool force = false)
        {
            UITab_Inventory.DragUI = dragImg;
            // ItemSlot.DragImg = dragImg;
            tabHeaderMenuNtoN.Reset();
            OnUiToggle?.Invoke(true);
            base.TryActivated(force);
        }

        public override void TryDeactivated(bool force = false)
        {
            OnUiToggle?.Invoke(false);
            base.TryDeactivated(force);
        }

        public override void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetGameKeyCode(Define.GameKey.Tab)))
            {
                CloseOwn();
                return;
            }
            tabHeaderMenuNtoN.KeyControl();
            base.KeyControl();
        }

        public override void GamePadControl()
        {
            if (InputManager.GetButtonDown(KeySettingManager.GetGameButton(Define.GameKey.Tab)))
            {
                CloseOwn();
                return;
            }
            tabHeaderMenuNtoN.GamePadControl();
            base.GamePadControl();
        }
    }
}