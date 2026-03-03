using System;
using Apis;
using Apis.UI;
using Apis.InvenSpace;
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
        
        [SerializeField] protected Image itemImg;
        [SerializeField] private Image changeImg;
        [SerializeField] private Image backgroundImg;
        [SerializeField] private Color frozenColor;
        [SerializeField] private Image equippedImg;

        [HideInInspector] public int index;
        // [HideInInspector] public InventoryGroup inventoryGroup;

        public InventoryList InventoryList { get; set; }
        // tab 메뉴에만 해당.(예외처리)
        // [HideInInspector] public UI_InventoryContent tabInventory;

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

        public virtual void OnSlotChanged(int ind, Item item)
        {
            if (ind != index) return;
            curItem = item;
            
            if (item != null)
            {
                itemImg.sprite = item.Image;
                itemImg.enabled = true;
                item.SaveData.slotIndex = ind;

                if (equippedImg != null)
                {
                    equippedImg.enabled = item.IsEquip;
                }
            }
            else
            {
                if (equippedImg != null)
                {
                    equippedImg.enabled = false;
                }
                itemImg.enabled = false;
            }
        }
        
        public override void FrozenToggle(bool isOn)
        {
            base.FrozenToggle(isOn);
            backgroundImg.color = isOn ? frozenColor : Color.white;
        }

        public void ChangedToggle(bool isOn)
        {
            if (changeImg == null) return;
            
            itemImg.color = isOn ? Color.white :Color.grey;
        }

        public void ToggleItemImg(bool isOn)
        {
            itemImg.enabled = isOn;
        }

        public bool CheckItemIndex(Item item)
        {
            return true;
        }

        #region DragSection

        

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            OnDragChanged?.Invoke(this, true);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            OnDragChanged?.Invoke(this, false);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            OnPointerChanged?.Invoke(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            OnPointerChanged?.Invoke(this, false);
        }
        
        #endregion
    }
}