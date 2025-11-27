using System;
using System.Collections.Generic;

namespace NewNewInvenSpace
{
    [Serializable]
    public enum GuitarInvenType
    {
        Growth,
        Etc
    }

    public class GuitarInventoryGroup
    {
        public Dictionary<GuitarInvenType, InventoryList> Invens;


        public GuitarInventoryGroup(int maxCnt, int cnt)
        {
            Invens = new Dictionary<GuitarInvenType, InventoryList>();
            foreach (GuitarInvenType type in Enum.GetValues(typeof(GuitarInvenType)))
                Invens.Add(type, new InventoryList(maxCnt, cnt));
        }

        public virtual bool Add(Item item, GuitarInvenType type)
        {
            if (!Invens.TryGetValue(type, out var inven)) return false;
            var ind = inven.GetEmptySlot();
            if (ind < 0) inven.Count += 1;
            ind = inven.GetEmptySlot();
            return inven.AddItem(ind, item);
        }

        // 기타 인벤 특성상, 고정 index로 넣는것 없다고 판단.
        // 잘못쓸수도 있으니 일단 주석 처리. 필요하다고 판단되는 경우, 주석 풀고 사용 필요.
        // 기타 인벤은 무조건 추가되어야 한다는 개념과 상반됨(해당 index에 아이템 미리 있으면 추가 불가능한 특성하고)
        /*
        public virtual bool Add(int index, Item item, GuitarInvenType type)
        {
            return Invens.TryGetValue(type, out var inven) && inven.AddItem(index, item);
        }
        */

        /*
        public bool IsFull(GuitarInvenType type)
        {
            return Invens.TryGetValue(type, out var inven) && inven.GetEmptySlot() < 0;
        }
        */

        public virtual Item Remove(int index, GuitarInvenType type)
        {
            if (!CheckInvenAndIndex(index, type, out var inven)) return null;
            var removalItem = inven.Remove(index);
            // 기타 인벤은 특성 상, 비어있는 곳이 없어야 함.
            for (var i = index + 1; i < inven.Count; i++)
            {
                // 더이상 오른쪽에서 옮길 item이 없는 경우
                if (inven[i] == null) break;
                inven.Change(i - 1, i);
            }

            return removalItem;
        }

        private bool CheckInvenAndIndex(int index, GuitarInvenType type, out InventoryList inventory)
        {
            if (0 <= index && Invens.TryGetValue(type, out var inventoryTemp) && index < inventoryTemp.Count)
            {
                inventory = inventoryTemp;
                return true;
            }

            inventory = null;
            return false;
        }

        public void AllClear()
        {
            foreach (var value in Invens) value.Value.Clear();
        }
    }
}