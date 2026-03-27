using System;
using Apis;

namespace Apis.InvenSpace
{
    public class InventoryList
    {
        public delegate void ItemChanged(int ind, Item item);

        public delegate void CountChanged(int cnt);


        public ItemChanged OnSlotChanged;
        public ItemChanged ItemAddedTo;
        public ItemChanged ItemRemovedFrom;
        
        public ItemChanged BeforeItemExcepted;
        public CountChanged OnCountChanged;
        
        public Item[] Slots;

        public InventoryList(int maxCnt, int cnt)
        {
            MaxCnt = maxCnt;
            Slots = new Item[cnt];
            _cnt = cnt;

            // 아이템을 획득했을 때 함수 호출
            ItemAddedTo += (_, item) => item.Collect();
            ItemRemovedFrom += (_, item) => item.Remove();
            // list에 들어오면 itemStorage 하위로 parent 설정.(inven storage하고는 다름)
            ItemAddedTo += (_, item) => item.SetParent(null);
            ItemRemovedFrom += (_, item) => item.SetParent(null);
            
            
        }

        public Item this[int index]
        {
            get => Slots[index];
            protected set
            {
                Slots[index] = value;
                // Debug.Log($"Inventory changed {index} {value?.Name}");
                OnSlotChanged?.Invoke(index, value);
            }
        }
        public int MaxCnt { get; }
        protected int _cnt;
        public int Count
        {
            get => _cnt;
            set
            {
                if (value == _cnt || value > MaxCnt) return;
                if (value < _cnt)
                {
                    for (int i = value; i < _cnt; i++)
                    {
                        if (this[i] != null)
                        {
                            BeforeItemExcepted?.Invoke(i, this[i]);
                            this[i] = null;
                        }
                    }
                }
                _cnt = value;
                Array.Resize(ref Slots, _cnt);
                OnCountChanged?.Invoke(_cnt);
            }
        }
        
        public int GetEmptySlot()
        {
            int ind = -1;
            for (int i = 0; i < _cnt; i++)
            {
                if (this[i] == null)
                {
                    ind = i;
                    break;
                }
            }
            return ind;
        }

        public bool IsEmpty()
        {
            int a = 0;
            foreach (Item item in Slots)
            {
                if (a >= _cnt) break;
                if (item != null)
                {
                    return false;
                }
                a++;
            }
            return true;
        }

        public bool IsFull()
        {
            int a = 0;
            foreach (Item item in Slots)
            {
                if (a >= _cnt) break;
                if (item == null)
                {
                    return false;
                }
                a++;
            }
            return true;
        }

        public virtual bool AddItem(Item item)
        {
            if (item == null) return false;
            for (int i = 0; i < Count; i++)
            {
                if (this[i] == null)
                {
                    this[i] = item;
                    ItemAddedTo?.Invoke(i, item);
                    return true;
                }
            }

            return false;
        }
        public virtual bool AddItem(int index, Item item)
        {
            if (index >= Count) return false;
            if (item == null) return false;
            if (this[index] != null) return false;
            this[index] = item;
            ItemAddedTo?.Invoke(index, item);
            return true;
        }

        public Item Remove(int index)
        {
            if (index >= Count) return null;
            Item item = this[index];
            if (item == null) return null;
            this[index] = null;
            ItemRemovedFrom?.Invoke(index, item);
            return InvenManager.instance.Storage.Get(item);
        }
        
        public int FindById(int itemId)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i] != null && this[i].ItemId == itemId)
                    return i;
            }
            return -1;
        }

        public void Change(int index1, int index2)
        {
            (this[index1], this[index2]) = (this[index2], this[index1]);
        }

        public void Clear()
        {
            for (int i = 0; i < _cnt; i++)
            {
                Remove(i)?.Return();
            }
        }
    }
}