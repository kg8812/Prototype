using System;
using Apis;
using Apis.UI;
using Apis.UI.Focus;
using Default;
using Managers;
using Apis.InvenSpace;
using UnityEngine;
using UnityEngine.UI;

namespace Apis
{
    public class UITab_Inventory: UI_InventoryContent 
    {
        public FocusParent Equipment;
        public FocusParent Inven;
        
        [SerializeField] private ItemSlot[] equipSlots;
        [SerializeField] private ItemSlot[] invenSlots;

        [SerializeField] private ScrollRect _scrollRect;

        
        protected virtual string itemSlotAddress => "ItemSlot";
        

        public static bool UsePrevent { get; set; } = true;
        public static bool PreventMovingOrigin { get; set; } = true;

        public static bool PreventMoving => PreventMovingOrigin && UsePrevent;

        private int _equipCnt;
        private int _storageCnt;

        protected InvenType CurInvenType;

        #region 초기화 관련

        public override void Init()
        {
            base.Init();
            
            Equipment.FocusGroup = this;
            Inven.FocusGroup = this;
            Equipment.InitCheck();
            Inven.InitCheck();
            
            _equipCnt = equipSlots.Length;
            _storageCnt = invenSlots.Length;
            
            // slot들 slot changed에 등록.
            SlotInit(equipSlots, InvenType.Equipment, _equipCnt);
            SlotInit(invenSlots, InvenType.Storage, _storageCnt);
            
            invenGroupManager.Invens[InvenType.Equipment].OnCountChanged +=
                (cnt) => SlotCntChanged(cnt, InvenType.Equipment);
            invenGroupManager.Invens[InvenType.Storage].OnCountChanged +=
                (cnt) => SlotCntChanged(cnt, InvenType.Storage);

            
            ChangeOn();
            
            
            ChangeFocusParent(Equipment);

            _scrollRect.UpdateFocusParentToScrollView(Inven);
        }

        private void SlotCntChanged(int cnt, InvenType type)
        {
            // Debug.Log($"slot cnt changed {cnt} {type}");
            int prevCnt = type == InvenType.Equipment ? _equipCnt : _storageCnt;
            FocusParent parent = type == InvenType.Equipment ? Equipment : Inven;
            if (type == InvenType.Equipment)
            {
                _equipCnt = cnt;
            }else if (type == InvenType.Storage)
            {
                _storageCnt = cnt;
            }
            if (cnt > prevCnt)
            {
                // add new slots
                ItemSlot[] newSlots = new ItemSlot[cnt-prevCnt];
                for (int i = 0; i < cnt-prevCnt; i++)
                {
                    newSlots[i] = GameManager.UI.MakeSubItem(itemSlotAddress, parent.transform) as ItemSlot;
                    // new slot position setting
                    parent.RegisterElement(newSlots[i]);
                }
                
                ItemSlot[] slots = type == InvenType.Equipment ? equipSlots : invenSlots;
                Array.Resize(ref slots, cnt);
                
                if (type == InvenType.Equipment) equipSlots = slots;
                else invenSlots = slots;
                
                for (int i = 0; i < cnt-prevCnt; i++)
                {
                    slots[i + prevCnt] = newSlots[i];
                }
                SlotInit(newSlots, type, cnt, prevCnt);
            }
            else
            {
                // delete origin slots
                ItemSlot[] slots = type == InvenType.Equipment ? equipSlots : invenSlots;
                for (int i = cnt; i < prevCnt; i++)
                {
                    ReturnSlot(slots[i], type);
                    parent.RemoveElement(slots[i]);
                    slots[i] = null;
                }
                Array.Resize(ref slots, cnt);
            }
        }
        
        protected virtual void SlotInit(ItemSlot[] slotList, InvenType invenType, int slotCnt, int startInd = 0)
        {
            int cnt = slotCnt;
            for (int i = 0; i < slotList.Length; i++)
            {
                ItemSlot slot = slotList[i];
                int myI = startInd + i;
                if (myI < cnt)
                {
                    slot.InitCheck();
                    slot.invenType = invenType;
                    slot.InventoryList = invenGroupManager.Invens[invenType];
                    slot.index = myI;
                    // slot.tabInventory = this;
                    slot.OnSelected.AddListener(() =>
                    {
                        CurInvenType = slot.invenType;
                    });
                    slot.OnDeselected.AddListener(() =>
                    {
                        if(curFocusedSlot == slot)
                        {
                            curFocusedSlot = null;
                        }
                    });
                    
                    slot.FocusOn += () =>
                    {
                        // Debug.Log($"curselected slot을 {slot.index}로 변경");
                        curFocusedSlot = slot;
                    };
                    invenGroupManager.Invens[invenType].OnSlotChanged += slot.OnSlotChanged;
                    
                    // drag 관련 세팅
                    slot.OnDragChanged = (iSlot, isOn) =>
                    {
                        if (isOn)
                            ItemSlotOnBeginDrag(iSlot);
                        else
                            ItemSlotOnEndDrag(iSlot);
                    };
                    slot.OnPointerChanged = ItemSlotOnPointerChanged;
                    
                    
                    slot.UpdateItem();
                }
                else
                {
                    ReturnSlot(slot, invenType);
                }
                
            }
        }

        private void ReturnSlot(ItemSlot itemSlot, InvenType type)
        {
            itemSlot.OnSelected?.RemoveAllListeners();
            itemSlot.OnDeselected?.RemoveAllListeners();
            invenGroupManager.Invens[type].OnSlotChanged -= itemSlot.OnSlotChanged;
            itemSlot.CloseOwn();
        }

        public void SetNavigationManager(IUI_NavigationManager manager)
        {
            NavigationManager = manager;
            Inven.NavigationManager = manager;
            Equipment.NavigationManager = manager;
        }
        #endregion
        
        

        public override void ChangeFocusParent(FocusParent fp)
        {
            base.ChangeFocusParent(fp);
            if (ReferenceEquals(fp, Equipment))
            {
                Inven.FocusReset();
            }
            else
            {
                Equipment.FocusReset();
            }
        }

        #region 탭오픈 여부

        public override void OnOpen()
        {
            base.OnOpen();

            _scrollRect.verticalScrollbar.value = 1;
            foreach (var eq in equipSlots)
            {
                eq.UpdateItem();
            }
            foreach (var iv in invenSlots)
            {
                iv.UpdateItem();
            }
        }

        public override void OnClose()
        {
            base.OnClose();
            CancelChange();
        }

        #endregion


        #region 컨트롤

        public override void KeyControl()
        {
            base.KeyControl();
            if (curFocusedSlot == null || PreventMoving) return;
            // if (InputManager.GetKeyDown(KeyCode.J))
            // {
            //     Debug.Log($"test: selected:{CurFocusedSlot?.index} change:{ChangeSlot?.index} Tochange:{ToChangeSlot?.index}");
            // }
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Equip)))
            {
                if (ReferenceEquals(curFocusedSlot.curItem, null)) return;
                if (IsChanging)
                {
                    // Debug.Log("equip");
                    SelectedForChange(curFocusedSlot);
                }
                else
                {
                    bool success;
                    if (CurInvenType == InvenType.Equipment)
                        success = invenGroupManager.MoveInvenType(curFocusedSlot.index, InvenType.Equipment,
                            InvenType.Storage);
                    else
                        success = invenGroupManager.MoveInvenType(curFocusedSlot.index, InvenType.Storage,
                            InvenType.Equipment);
                    if (!success)
                    {
                        curFocusedSlot.ChangedToggle(true);
                        TryChangeDiffInvenType(curFocusedSlot);
                    }
                }
            }else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                if (IsChanging)
                {
                    // Debug.Log("select");
                    SelectedForChange(curFocusedSlot);
                }
                else
                {
                    if (ReferenceEquals(curFocusedSlot.curItem, null)) return;
                    curFocusedSlot.ChangedToggle(true);
                    TryChange(curFocusedSlot);
                }
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            if (curFocusedSlot == null || PreventMoving) return;
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Equip)))
            {
                if (ReferenceEquals(curFocusedSlot.curItem, null)) return;
                if (IsChanging)
                {
                    
                    SelectedForChange(curFocusedSlot);
                }
                else
                {
                    bool success;
                    if (CurInvenType == InvenType.Equipment)
                        success = invenGroupManager.MoveInvenType(curFocusedSlot.index, InvenType.Equipment,
                            InvenType.Storage);
                    else
                        success = invenGroupManager.MoveInvenType(curFocusedSlot.index, InvenType.Storage,
                            InvenType.Equipment);
                    if (!success)
                    {
                        // Debug.Log("f키 안되서 try change");
                        curFocusedSlot.ChangedToggle(true);
                        TryChangeDiffInvenType(curFocusedSlot);
                    }
                }
            }else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                if (IsChanging)
                {
                    SelectedForChange(curFocusedSlot);
                }
                else
                {
                    if (ReferenceEquals(curFocusedSlot.curItem, null)) return;
                    curFocusedSlot.ChangedToggle(true);
                    TryChange(curFocusedSlot);
                }
            }
        }

        #endregion


        #region 드래그 관련

        public bool IsDragging { get; private set; }
        public static UI_DragItem DragUI { get; set; }
        public ItemSlot ToChangeSlot { get; private set; }
        
        private bool TryDrag(Item item)
        {
            if (IsDragging || PreventMoving) return false;
            DragUI.DragImg.sprite = item.Image;
            IsDragging = true;
            DragUI.DragOn();
            return true;
        }

        private void ItemSlotOnBeginDrag(ItemSlot slot)
        {
            if (slot.curItem == null) return;
            if (TryDrag(slot.curItem))
            {
                slot.SelectOn();
                // FrozenToggle(true);
                ToChangeSlot = slot;
                slot.ToggleItemImg(false);
            }
        }
        
        private void ItemSlotOnEndDrag(ItemSlot slot)
        {
            if (!IsDragging) return;
            IsDragging = false;
            DragUI.DragOff();
            if (ToChangeSlot != null || PreventMoving)
            {
                if (!ReferenceEquals(ToChangeSlot, slot))
                {
                    if (ToChangeSlot.curItem != null)
                    {
                        SystemManager.SystemAlert("이미 장착중인 슬롯입니다",null);
                    }
                    else if (!ToChangeSlot.CheckItemIndex(slot.curItem))
                    {
                        SystemManager.SystemAlert("원래 위치에 넣어주세요",null);
                    }
                    else if (invenGroupManager.Change(slot.index, slot.invenType, ToChangeSlot.index, ToChangeSlot.invenType))
                    {
                        slot.SelectOff(true);
                        ToChangeSlot.SelectOn();
                        return;
                    }
                }
            }
            else
            {
                Debug.Log("failed: change slot by drag - no slot");
            }
            slot.ToggleItemImg(true);
            slot.SelectOn();
        }

        private void ItemSlotOnPointerChanged(ItemSlot slot, bool isStart)
        {
            if (isStart)
            {
                if (IsDragging)
                {
                    ToChangeSlot = slot;
                }
            }
            else
            {
                if (IsDragging && ToChangeSlot == slot)
                {
                    ToChangeSlot = null;
                }
            }
        }

        #endregion


        public override void TryChange(ItemSlot slot)
        {
            base.TryChange(slot);
            Equipment.ChangeFocusType(false);
            Inven.ChangeFocusType(false);
        }

        public void TryChangeDiffInvenType(ItemSlot slot)
        {
            IsChanging = true;
            ChangeSlot = slot;
            ChangeOff();
            (ChangeSlot.invenType == InvenType.Storage ? Inven : Equipment).FocusReset();
            (ChangeSlot.invenType == InvenType.Storage ? Inven : Equipment).AllDisableToggle(true);
            (ChangeSlot.invenType == InvenType.Storage ? Equipment : Inven).ChangeCanNoneFocus(false);
            (ChangeSlot.invenType == InvenType.Storage ? Equipment : Inven).ChangeFocusType(false);
            (ChangeSlot.invenType == InvenType.Storage ? Equipment : Inven).MoveTo(0);
            ChangeFocusParent(ChangeSlot.invenType == InvenType.Storage ? Equipment : Inven);
        }


        public override bool CancelChange()
        {
            if (!base.CancelChange()) return false;
            Equipment.ChangeFocusType(true);
            Inven.ChangeFocusType(true);
            // (_changeOriginType == InvenType.Storage ? Equipment : Inven).MoveTo(slot.index);
            (ChangeSlot.invenType == InvenType.Storage ? Equipment : Inven).ChangeCanNoneFocus(true);
            (ChangeSlot.invenType == InvenType.Storage ? Inven : Equipment).AllDisableToggle(false);
            (ChangeSlot.invenType == InvenType.Storage ? Inven : Equipment).FocusReset();
            ChangeOn();
            return true;
        }


        protected virtual void ChangeOn()
        {
            Equipment.tableData.moveRight = MoveToInven;
            
            Inven.tableData.moveLeft = MoveToEquipment;
        }
        protected virtual void ChangeOff()
        {
            Equipment.tableData.moveUp = null;
            Equipment.tableData.moveDown = null;
            
            Inven.tableData.moveUp = null;
            Inven.tableData.moveDown = null;
        }

        public override bool IsAtBoundary(NavigationDirection direction)
        {
            if(_curFocus == null) return true;
            if (!_curFocus.IsAtBoundary(direction)) return false;

            if (_curFocus == Equipment && direction == NavigationDirection.Right) return false;
            if(_curFocus == Inven && direction == NavigationDirection.Left) return false;

            return true;
        }

        public override void ResetFocus()
        {
            base.ResetFocus();
            ChangeFocusParent(Equipment);
        }

        #region KeyBoardMoveSection

        protected virtual void MoveToInven(UIElement current)
        {
            Vector2 curPos = current.rectTransform.TransformPoint(current.rectTransform.rect.center);

            ItemSlot best = null;
            float bestDist = float.MaxValue;

            foreach (var el in Inven.focusList)
            {
                RectTransform rect = el.rectTransform;
                float dx = Vector2.Distance(rect.TransformPoint(rect.rect.center), curPos);

                if (dx < bestDist)
                {
                    bestDist = dx;
                    best = el as ItemSlot;
                }
            }

            if (best != null)
            {
                Inven.MoveTo(Inven.focusList.IndexOf(best));
                ChangeFocusParent(Inven);
            }
        }

        protected virtual void MoveToEquipment(UIElement current)
        {
            
            Vector2 curPos = current.rectTransform.TransformPoint(current.rectTransform.rect.center);

            ItemSlot best = null;
            float bestDist = float.MaxValue;

            foreach (var el in Equipment.focusList)
            {
                RectTransform rect = el.rectTransform;
                float dx = Vector2.Distance(rect.TransformPoint(rect.rect.center), curPos);

                if (dx < bestDist)
                {
                    bestDist = dx;
                    best = el as ItemSlot;
                }
            }

            if (best != null)
            {
                Equipment.MoveTo(Equipment.focusList.IndexOf(best));
                ChangeFocusParent(Equipment);
            }
        }

        #endregion
    }
}