using Apis.Util;

namespace Apis.DataType
{
    [System.Serializable]
    public class DropDataType
    {
        public int[] dropGroups;
        public int[] dropGroupChances;
    }
    

    [System.Serializable]
    public class DropGroupDataType: HasChance
    {
        public int dropGroup;
        public int dropItemType;
        public int dropItemIndex;
        public int chance { get; set; }
        public int[] amount;
    }
}
