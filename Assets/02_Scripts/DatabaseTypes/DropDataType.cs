using System;
using Apis.Util;

namespace Apis.DataType
{
    [Serializable]
    public class DropDataType
    {
        public int[] dropGroups;
        public int[] dropGroupChances;
    }


    [Serializable]
    public class DropGroupDataType : HasChance
    {
        public int dropGroup;
        public int dropItemType;
        public int dropItemIndex;
        public int[] amount;
        public int chance { get; set; }
    }
}