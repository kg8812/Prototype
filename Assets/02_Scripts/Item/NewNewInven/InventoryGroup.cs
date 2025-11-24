using System.Collections.Generic;
using UnityEngine;

namespace NewNewInvenSpace
{
    public enum InvenType
    {
        Equipment, Storage, Hidden
    }
    public class InventoryGroup
    {
        protected PresetType _presetType;
        public virtual PresetType PresetType
        {
            get => _presetType;
            set => _presetType = value;
        }
        public Dictionary<InvenType, InventoryList> Invens;

        public InventoryGroup(int eqMaxCnt, int eqCnt, int stMaxCnt, int stCnt)
        {
            Invens = new();
            Invens.Add(InvenType.Equipment, new InventoryList(eqMaxCnt, eqCnt));
            Invens.Add(InvenType.Storage, new InventoryList(stMaxCnt, stCnt));
            
            Invens[InvenType.Equipment].BeforeItemExcepted = (int ind, Item item) =>
            {
                if (!MoveInvenType(ind, InvenType.Equipment, InvenType.Storage))
                {
                    Abandon(ind, InvenType.Equipment);
                }
            };
            Invens[InvenType.Storage].BeforeItemExcepted = (int ind, Item item) => Abandon(ind, InvenType.Storage);
        }
        
        public virtual bool Add(Item item, InvenType type)
        {
            if(!Invens.TryGetValue(type, out var inven)) return false;
            int ind = inven.GetEmptySlot();
            if (ind < 0) return false;
            return inven.AddItem(ind, item);
        }
        
        public virtual bool Add(int index, Item item, InvenType type)
        {
            return Invens.TryGetValue(type, out var inven) && inven.AddItem(index, item);
        }

        public bool IsFull(InvenType type)
        {
            return Invens.TryGetValue(type, out var inven) && inven.GetEmptySlot() < 0;
        }

        public bool MoveInvenType(int fromInd, InvenType fromType, InvenType toType)
        {
            if (!Invens.TryGetValue(toType, out var toInven)) return false;
            int toInd = toInven.GetEmptySlot();
            // empty slot이 없는 경우
            if (toInd < 0) return false;

            return Change(fromInd, fromType, toInd, toType);
        }

        public bool MoveInvenTypeAll(InvenType fromType, InvenType toType)
        {
            if (!Invens.TryGetValue(fromType, out var fromInven)) return false;
            for (int i = 0; i < fromInven.Count; i++)
            {
                if (!MoveInvenType(i, fromType, toType)) return false;
            }
            return true;
        }

        protected virtual bool ChangeCheck(int index1, InvenType type1, int index2, InvenType type2)
        {
            return true;
        }
        
        public virtual bool Change(int index1, InvenType type1, int index2, InvenType type2)
        {
            // 로직 상, index1 -> index2 / index2 -> index1로 교체가 이루어지기 때문에
            // equipment에서 먼저 빼주는 걸 선행하기 위함.
            if (type1 == InvenType.Storage && type2 == InvenType.Equipment)
                return Change(index2, type2, index1, type1);

            if (!ChangeCheck(index1, type1, index2, type2)) return false;
            
            // Debug.Log($"chagne try {index1} {type1} {index2} {type2}");
            if (!CheckInvenAndIndex(index1, type1, out var inventory1)) return false;
            // Debug.Log($"chagne try2");
            if (type1 == type2)
            {
                // Debug.Log($"chagne try same");
                inventory1.Change(index1, index2);
                return true;
            }
            else
            {
                // Debug.Log($"chagne try diff");
                if (!CheckInvenAndIndex(index2, type2, out var inventory2)) return false;

                Item itemFrom1 = inventory1.Remove(index1);
                Item itemFrom2 = inventory2.Remove(index2);
                
                inventory1.AddItem(index1, itemFrom2);
                inventory2.AddItem(index2, itemFrom1);
                return true;
            }
        }

        public virtual Item Abandon(int index, InvenType type)
        {
            if (!CheckInvenAndIndex(index, type, out var inven)) return null;
            return inven.Remove(index);
        }
        
        
        private bool CheckInvenAndIndex(int index, InvenType type, out InventoryList inventory)
        {
            if (0 <= index && Invens.TryGetValue(type, out var inventoryTemp) && index < inventoryTemp.Count)
            {
                inventory = inventoryTemp;
                return true;
            }
            inventory = null;
            return false;
        }
    }
}