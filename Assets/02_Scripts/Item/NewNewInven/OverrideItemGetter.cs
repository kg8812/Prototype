using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public class OverrideItemGetter
    {
        private static readonly Dictionary<int, Item> _overrideItem = new();
        private readonly ItemStorage _overrideItemStorage;
        public ItemStorage OverrideItemStorage => _overrideItemStorage;
        
        public OverrideItemGetter()
        {
            _overrideItemStorage = new ItemStorage("OverrideItemStorage");
        }

        

        /// <summary>
        /// 그냥 item ref만 가져옴. storage 꺼내기 x
        /// </summary>
        public Item GetItem(int itemId)
        {
            return _overrideItem.GetValueOrDefault(itemId);
        }

        public Item GetItemFromStorage(int itemId)
        {
            if (_overrideItem.TryGetValue(itemId, out var value))
            {
                return _overrideItemStorage.Get(value);
            }

            return null;
        }

        public void RegisterExternalItem(Item item)
        {
            if (item == null) return;
            _overrideItemStorage.Store(item);
            _overrideItem.Add(item.ItemId, item);
            item.OnUnEquipped -= SetItemStorage;
            item.OnUnEquipped += SetItemStorage;
        }
        
        public Item AddNewOverrideItem(int skillItemId)
        {
            if (skillItemId == 0) return null;
            
            if (_overrideItem.TryGetValue(skillItemId, out var overrideItem)) return overrideItem;
            
            Item item = CreateNewOverrideItem(skillItemId);
            if (item == null) return null;
            _overrideItemStorage.Store(item);
            _overrideItem.Add(skillItemId, item);
            return item;
        }
        
        private Item CreateNewOverrideItem(int itemId)
        {
            if (itemId == 0) return null;
            Item newItem = GameManager.Item.GetWeapon(itemId);
            if (newItem == null)
                newItem = GameManager.Item.GetActiveSkill(itemId);
            if (newItem == null)
            {
                Debug.Log(itemId);
                Debug.LogError("해당 아이템이 존재하지 않음.");
                return null;
            }

            newItem.OnUnEquipped -= SetItemStorage;
            newItem.OnUnEquipped += SetItemStorage;
            return newItem;
        }
        
        private void SetItemStorage(Item item)
        {
            _overrideItemStorage.Store(item);
        }
    }
}