using System;

namespace Apis.DataType
{
    [Serializable]
    public class AccessoryDataType
    {
        public int accId;
        public int accName;
        public int accFlavorText;
        public int accDesc;
        public int grade;
        public string iconPath;
        public bool unlock;
        public int cost;
    }
}