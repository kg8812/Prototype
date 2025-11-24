using System;
using System.Collections.Generic;
using System.Linq;
using Apis.DataType;
using Apis.Util;
using Save.Schema;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Apis
{
    public struct DropItemTypeInGroup : HasChance
    {
        public int dropItemType;
        public int dropItemIndex;
        public int chance { get; set; }
        public int[] amount;

        public DropItemTypeInGroup(int dropItemType, int dropItemIndex, int chance, int[] amount)
        {
            this.dropItemType = dropItemType;
            this.dropItemIndex = dropItemIndex;
            this.chance = chance;
            this.amount = amount;
        }
    }

    public class DropSystem
    {
        private readonly Dictionary<int, List<DropItemTypeInGroup>> _dropItemsPerGroup;
        private readonly Dictionary<string, DropDataType> _dropDataTable;

        public DropSystem(Dictionary<int, List<DropItemTypeInGroup>> dropItemsPerGroup)
        {
            _dropItemsPerGroup = dropItemsPerGroup;
            _dropDataTable = GameManager.Data.GetDataTable<DropDataType>(DataTableType.Drop);
        }

        private List<DropItem> GetDropItem(int dropGroupIndex)
        {
            if (_dropItemsPerGroup.TryGetValue(dropGroupIndex, out List<DropItemTypeInGroup> value))
            {
                var chanceList = Default.Utils.GetChanceList<DropItemTypeInGroup>(value).Where(x =>
                    x.dropItemType - 1 is not ((int)DropItemType.Accessory or (int)DropItemType.Weapon) ||
                    DataAccess.Codex.IsOpen(CodexData.CodexType.Item, x.dropItemIndex)).ToList();
                //var chanceList = GGDok.Utils.GetChanceList<DropItemTypeInGroup>(value);

                int randInd = Random.Range(0, chanceList.Count);
                int amount = Random.Range(chanceList[randInd].amount[0], chanceList[randInd].amount[1] + 1);
                return DropItemFactoryManager.instance.GetDropItemById(chanceList[randInd].dropItemType,
                    chanceList[randInd].dropItemIndex, amount);
            }
            else
            {
                throw new Exception($"{dropGroupIndex}번 dropGroup은 존재하지 않습니다");
            }
        }

        public List<DropItem> GetDropItems(int dropperIndex)
        {
            if (_dropDataTable.TryGetValue(dropperIndex.ToString(), out DropDataType value))
            {
                List<DropItem> dropItems = new List<DropItem>();
                for (int i = 0; i < value.dropGroupChances.Length; i++)
                {
                    // chance check
                    if (!(Random.Range(0, 1000) < value.dropGroupChances[i])) continue;
                    dropItems.AddRange(GetDropItem(value.dropGroups[i]));
                }

                return dropItems;
            }
            else
            {
                Debug.LogError("상자에 입력한 dropperId가 현재 데이터테이블에 존재하지 않습니다");
                return new List<DropItem>();
            }
        }
    }
}