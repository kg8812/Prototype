using System;
using Apis;
using Apis.UI;
using NewNewInvenSpace;
using UI;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Apis
{
    public class ItemSlot: UIAsset_Toggle
    {
        // public static bool IsDragging;
        // public static UI_DragItem DragImg;
        // public static ItemSlot ToChangeSlot;
        
        [SerializeField] private Image itemImg;
        [SerializeField] private Image changeImg;
        [SerializeField] private Image backgroundImg;
        [SerializeField] private Color frozenColor;

        [HideInInspector] public int index;
        // [HideInInspector] public InventoryGroup inventoryGroup;

        public InventoryList InventoryList { get; set; }
        // tab 메뉴에만 해당.(예외처리)
        // [HideInInspector] public UI_InventoryContent tabInventory;

        [SerializeField] private bool hasEquipView = true;
        public InvenType invenType;
        [HideInInspector] public Item curItem;

        private Guid _dragGuid;

        // true = start / false = end
        public Action<ItemSlot, bool> OnDragChanged;
        public Action<ItemSlot, bool> OnPointerChanged;


        public void UpdateItem()
        {
            OnSlotChanged(index, InventoryList[index]);
        }

        public void OnSlotChanged(int ind, Item item)
        {
            if (ind != index) return;
            curItem = item;
            
            if (item != null)
            {
                itemImg.sprite = item.Image;
                itemImg.enabled = true;
                item.slot = this;
                item.SaveData.slotIndex = ind;
            }
            else
            {
                itemImg.enabled = false;
            }
        }

        public override void KeyControl()
        {
            /*
            if (isFrozen) return;
            if (hasEquipView && InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Equip)))
            {
                if (ReferenceEquals(curItem, null)) return;
                if (tabInventory.IsChanging)
                {
                    tabInventory.SelectedForChange(this);
                }
                else
                {
                    if (!(invenType == InvenType.Equipment
                            ? inventoryGroup.MoveInvenType(index, InvenType.Equipment, InvenType.Storage)
                            : inventoryGroup.MoveInvenType(index, InvenType.Storage, InvenType.Equipment)))
                    {
                        Debug.Log("f키 안되서 try change");
                        ChangedToggle(true);
                        tabInventory.TryChangeDiffInvenType(this);
                    }
                }
            }else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Select)))
            {
                if (tabInventory.IsChanging)
                {
                    tabInventory.SelectedForChange(this);
                }
                else
                {
                    if (ReferenceEquals(curItem, null)) return;
                    ChangedToggle(true);
                    tabInventory.TryChange(this);
                }
                
            }
            */
        }

        public override void GamePadControl()
        {
            /*
            if (isFrozen) return;
            if (hasEquipView && InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Equip)))
            {
                if (ReferenceEquals(curItem, null)) return;
                if (tabInventory.IsChanging)
                {
                    tabInventory.SelectedForChange(this);
                }
                else
                {
                    if (!(invenType == InvenType.Equipment
                            ? inventoryGroup.MoveInvenType(index, InvenType.Equipment, InvenType.Storage)
                            : inventoryGroup.MoveInvenType(index, InvenType.Storage, InvenType.Equipment)))
                    {
                        Debug.Log("f키 안되서 try change");
                        ChangedToggle(true);
                        tabInventory.TryChangeDiffInvenType(this);
                    }
                }
            }else if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Select)))
            {
                if (tabInventory.IsChanging)
                {
                    tabInventory.SelectedForChange(this);
                }
                else
                {
                    if (ReferenceEquals(curItem, null)) return;
                    ChangedToggle(true);
                    tabInventory.TryChange(this);
                }
                
            }
            */
        }
        public override void FrozenToggle(bool isOn)
        {
            base.FrozenToggle(isOn);
            backgroundImg.color = isOn ? frozenColor : Color.white;
        }

        public void ChangedToggle(bool isOn)
        {
            changeImg.enabled = isOn;
        }

        public void ToggleItemImg(bool isOn)
        {
            itemImg.enabled = isOn;
        }


        #region DragSection

        

        public override void OnBeginDrag(PointerEventData eventData)
        {
            OnDragChanged?.Invoke(this, true);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            OnDragChanged?.Invoke(this, false);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerChanged?.Invoke(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            OnPointerChanged?.Invoke(this, false);
        }
        
        #endregion
    }
}