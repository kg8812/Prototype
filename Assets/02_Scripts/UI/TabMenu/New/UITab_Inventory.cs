using System;
using Apis;
using Apis.UI.Focus;
using NewNewInvenSpace;
using UI;
using UnityEngine;

namespace Apis
{
    public class UITab_Inventory: UI_InventoryContent
    {
        public FocusParent Equipment;
        public FocusParent Inven;
        
        [SerializeField] private ItemSlot[] equipSlots;
        [SerializeField] private ItemSlot[] invenSlots;

        [SerializeField] protected ItemDescription description;

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

            int nextEqCnt = invenGroupManager.Invens[InvenType.Equipment].Count;
            int nextStCnt = invenGroupManager.Invens[InvenType.Storage].Count;

            

            if (nextEqCnt != _equipCnt)
                SlotCntChanged(nextEqCnt, InvenType.Equipment);
            if (nextStCnt != _storageCnt)
                SlotCntChanged(nextStCnt, InvenType.Storage);
            
            invenGroupManager.Invens[InvenType.Equipment].OnCountChanged +=
                (cnt) => SlotCntChanged(cnt, InvenType.Equipment);
            invenGroupManager.Invens[InvenType.Storage].OnCountChanged +=
                (cnt) => SlotCntChanged(cnt, InvenType.Storage);

            
            ChangeOn();
            
            
            Equipment.MoveTo(0);
            _curFocus = Equipment;
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
                    newSlots[i] = GameManager.UI.MakeSubItem("ItemSlot", parent.transform) as ItemSlot;
                    // new slot position setting
                    parent.RegisterElement(newSlots[i]);
                }
                
                ItemSlot[] slots = type == InvenType.Equipment ? equipSlots : invenSlots;
                Array.Resize(ref slots, cnt);
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
                    slot.OnValueChanged.AddListener(isOn =>
                    {
                        if (isOn)
                        {
                            description.ChangeInfo(slot.curItem);
                            CurInvenType = slot.invenType;
                        }
                        else if(CurFocusedSlot == slot)
                        {
                            CurFocusedSlot = null;
                        }
                    });
                    slot.FocusOn += () =>
                    {
                        // Debug.Log($"curselected slot을 {slot.index}로 변경");
                        CurFocusedSlot = slot;
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
            itemSlot.OnValueChanged?.RemoveAllListeners();
            invenGroupManager.Invens[type].OnSlotChanged -= itemSlot.OnSlotChanged;
            itemSlot.CloseOwn();
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
            if (CurFocusedSlot == null || PreventMoving) return;
            // if (InputManager.GetKeyDown(KeyCode.J))
            // {
            //     Debug.Log($"test: selected:{CurFocusedSlot?.index} change:{ChangeSlot?.index} Tochange:{ToChangeSlot?.index}");
            // }
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Equip)))
            {
                if (ReferenceEquals(CurFocusedSlot.curItem, null)) return;
                if (IsChanging)
                {
                    // Debug.Log("equip");
                    SelectedForChange(CurFocusedSlot);
                }
                else
                {
                    bool success;
                    if (CurInvenType == InvenType.Equipment)
                        success = invenGroupManager.MoveInvenType(CurFocusedSlot.index, InvenType.Equipment,
                            InvenType.Storage);
                    else
                        success = invenGroupManager.MoveInvenType(CurFocusedSlot.index, InvenType.Storage,
                            InvenType.Equipment);
                    if (!success)
                    {
                        // Debug.Log("f키 안되서 try change");
                        CurFocusedSlot.ChangedToggle(true);
                        TryChangeDiffInvenType(CurFocusedSlot);
                    }
                }
            }else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                if (IsChanging)
                {
                    // Debug.Log("select");
                    SelectedForChange(CurFocusedSlot);
                }
                else
                {
                    if (ReferenceEquals(CurFocusedSlot.curItem, null)) return;
                    CurFocusedSlot.ChangedToggle(true);
                    TryChange(CurFocusedSlot);
                }
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            if (CurFocusedSlot == null || PreventMoving) return;
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Equip)))
            {
                if (ReferenceEquals(CurFocusedSlot.curItem, null)) return;
                if (IsChanging)
                {
                    
                    SelectedForChange(CurFocusedSlot);
                }
                else
                {
                    bool success;
                    if (CurInvenType == InvenType.Equipment)
                        success = invenGroupManager.MoveInvenType(CurFocusedSlot.index, InvenType.Equipment,
                            InvenType.Storage);
                    else
                        success = invenGroupManager.MoveInvenType(CurFocusedSlot.index, InvenType.Storage,
                            InvenType.Equipment);
                    if (!success)
                    {
                        // Debug.Log("f키 안되서 try change");
                        CurFocusedSlot.ChangedToggle(true);
                        TryChangeDiffInvenType(CurFocusedSlot);
                    }
                }
            }else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                if (IsChanging)
                {
                    SelectedForChange(CurFocusedSlot);
                }
                else
                {
                    if (ReferenceEquals(CurFocusedSlot.curItem, null)) return;
                    CurFocusedSlot.ChangedToggle(true);
                    TryChange(CurFocusedSlot);
                }
            }
        }

        #endregion


        #region 드래그 관련

        public bool IsDragging { get; private set; }
        public static UI_DragItem DragUI { get; set; }
        public ItemSlot ToChangeSlot { get; private set; }
        private Guid _dragGuid;
        
        private bool TryDrag(Item item)
        {
            if (IsDragging || PreventMoving) return false;
            DragUI.DragImg.sprite = item.Image;
            IsDragging = true;
            DragUI.TryActivated();
            return true;
        }

        private void ItemSlotOnBeginDrag(ItemSlot slot)
        {
            if (slot.curItem == null) return;
            if (TryDrag(slot.curItem))
            {
                slot.SelectOn();
                // FrozenToggle(true);
                _dragGuid = GameManager.instance.PreventControlOn();
                ToChangeSlot = slot;
                slot.ToggleItemImg(false);
            }
        }
        
        private void ItemSlotOnEndDrag(ItemSlot slot)
        {
            if (!IsDragging) return;
            IsDragging = false;
            DragUI.TryDeactivated();
            GameManager.instance.PreventControlOff(_dragGuid);
            if (ToChangeSlot != null || PreventMoving)
            {
                if (!ReferenceEquals(ToChangeSlot, slot))
                {
                    if (invenGroupManager.Change(slot.index, slot.invenType, ToChangeSlot.index, ToChangeSlot.invenType))
                    {
                        slot.SelectOff(true);
                        ToChangeSlot.SelectOn();
                        return;
                    }
                }
                else if (slot.curItem != null)
                {
                    Debug.Log("failed: change slot by drag - its me");
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
            _curFocus = (ChangeSlot.invenType == InvenType.Storage ? Equipment : Inven);
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
            Equipment.tableData.moveUp = MoveUpOnEquipment;
            Equipment.tableData.moveDown = MoveDownOnEquipment;
            
            Inven.tableData.moveUp = MoveUpOnInven;
            Inven.tableData.moveDown = MoveDownOnInven;
        }
        protected virtual void ChangeOff()
        {
            Equipment.tableData.moveUp = null;
            Equipment.tableData.moveDown = null;
            
            Inven.tableData.moveUp = null;
            Inven.tableData.moveDown = null;
        }
        
        
        #region KeyBoardMoveSection

        protected virtual void MoveUpOnEquipment(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Equipment.tableData.x, Inven.tableData.x);
            
            // 해당하는 가장 밑의 개체로 이동.
            Inven.MoveTo(realX + ((Inven.focusList.Count - realX - 1) / Inven.tableData.x) * Inven.tableData.x );
            _curFocus = Inven;
        }

        protected virtual void MoveDownOnEquipment(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Equipment.tableData.x, Inven.tableData.x);
            Inven.MoveTo(realX);
            _curFocus = Inven;
        }

        protected virtual void MoveUpOnInven(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Inven.tableData.x, Equipment.tableData.x);
            // 해당하는 가장 밑의 개체로 이동.
            Equipment.MoveTo(realX + ((Equipment.focusList.Count - realX - 1) / Equipment.tableData.x) * Equipment.tableData.x );
            _curFocus = Equipment;
        }

        protected virtual void MoveDownOnInven(int x)
        {
            int realX = Default.FormatUtils.GetRatioIntByInt(x, Inven.tableData.x, Equipment.tableData.x);
            Equipment.MoveTo(realX);
            _curFocus = Equipment;
        }

        #endregion
    }
}