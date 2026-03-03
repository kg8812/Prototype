using Apis.UI.Focus;
using Apis.InvenSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    public class UI_InventoryContent: UI_FocusContent, IFocusGroup , IUI_Navigatable
    {
        protected FocusParent _curFocus;

        protected virtual InventoryGroup invenGroupManager { get; }
        
        public bool IsChanging { get; protected set; }

        protected ItemSlot ChangeSlot;
        // 등록하는 event 자체는 여기서 관리 x
        // 외부 스크립트에서 설정 event 달아줘야 함
        protected ItemSlot curFocusedSlot;
        public ItemSlot CurFocusedSlot => curFocusedSlot;
        
        public override void KeyControl()
        {
            if (IsChanging)
            {
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Cancel)))
                {
                    CancelChange();
                    return;
                }
            }
            _curFocus?.KeyControl();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            if (IsChanging)
            {
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Cancel)))
                {
                    CancelChange();
                    return;
                }
            }
            _curFocus?.GamePadControl();
        }

        public override void OnOpen()
        {
            base.OnOpen();
            ResetFocus();
        }

        public override void OnClose()
        {
            base.OnClose();
            ResetFocus();
        }
        

        public virtual void ChangeFocusParent(FocusParent fp)
        {
            _curFocus = fp;
        }

        public virtual void TryChange(ItemSlot slot)
        {
            IsChanging = true;
            ChangeSlot = slot;
        }
        
        
        public virtual void SelectedForChange(ItemSlot slot)
        {
            if (!IsChanging) return;
            CancelChange();
            if (ReferenceEquals(slot, ChangeSlot)) return;
            if (invenGroupManager.Change(ChangeSlot.index, ChangeSlot.invenType, slot.index, slot.invenType))
            {
                slot.SelectOn();
            }
            else
            {
                ChangeSlot.SelectOn();
            }
        }
        
        
        public virtual bool CancelChange()
        {
            if (!IsChanging) return false;
            IsChanging = false;
            ChangeSlot?.ChangedToggle(false);
            return true;
        }

        public void OnNavigatedTo()
        {
            _curFocus?.OnNavigatedTo();
        }

        public void OnNavigatedFrom()
        {
            _curFocus?.OnNavigatedFrom();
        }

        public IUI_NavigationManager NavigationManager { get; set; }

        public virtual bool IsAtBoundary(NavigationDirection direction)
        {
            if (_curFocus == null) return true;
            
            return _curFocus.IsAtBoundary(direction);
        }

        public virtual void ResetFocus()
        {
            _curFocus?.FocusReset();
        }
    }
}