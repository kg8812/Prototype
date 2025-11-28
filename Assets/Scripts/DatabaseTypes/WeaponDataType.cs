using System;

namespace Apis.DataType
{
    [Serializable]
    public class WeaponDataType
    {
        // public int index;
        public int weaponId;
        public int weaponNameString;
        public int description;
        public float attackPower;

        public string iconPath;
        // public int price;
    }

    [Serializable]
    public class MotionGroupDataType
    {
        public int index;
        public int[] groundMotions;
        public int[] airMotions;
        public int[] groundColliders;
        public int[] airColliders;
        public int[] groundLegMotions;
        public int[] airLegMotions;
    }

    [Serializable]
    public class MotionDataType
    {
        public int index;
        public string motionName;
    }
}