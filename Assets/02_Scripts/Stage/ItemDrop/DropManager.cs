using System.Collections.Generic;
using Apis.DataType;

namespace Apis
{
    public class DropManager
    {
        private DropSystem _dropSystem;


        public void Init()
        {
            InitDropManager();
        }


        private void InitDropManager()
        {
            Dictionary<int, List<DropItemTypeInGroup>> dropItemsPerGroup =
                new Dictionary<int, List<DropItemTypeInGroup>>();

            foreach (KeyValuePair<string, DropGroupDataType> value in
                     GameManager.Data.GetDataTable<DropGroupDataType>(DataTableType.DropGroup))
            {
                DropItemTypeInGroup dropItemInGroup = new DropItemTypeInGroup(value.Value.dropItemType,
                    value.Value.dropItemIndex, value.Value.chance, value.Value.amount);
                if (dropItemsPerGroup.ContainsKey(value.Value.dropGroup))
                {
                    dropItemsPerGroup[value.Value.dropGroup].Add(dropItemInGroup);
                }
                else
                {
                    List<DropItemTypeInGroup> newDropGroup = new List<DropItemTypeInGroup>() { dropItemInGroup };
                    dropItemsPerGroup.Add(value.Value.dropGroup, newDropGroup);
                }
            }

            _dropSystem = new DropSystem(dropItemsPerGroup);
        }

        public List<DropItem> GetDropItems(int dropperIndex)
        {
            return _dropSystem.GetDropItems(dropperIndex);
        }
    }
}