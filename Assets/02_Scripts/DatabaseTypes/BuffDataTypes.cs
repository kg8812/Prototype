using System;
using Apis.Util;

namespace Apis.DataType
{
    [Serializable]
    public class SubBuffOptionDataType
    {
        public int name;
        public int description;
        public int maxStack;
        public float duration;
        public int stackDecrease;
        public float[] amount;
        public bool showIcon;
        public string iconPath;
    }

    [Serializable]
    public class BuffDataType
    {
        public int index;
        public int buffName;
        public int buffDesc;
        public int buffMainType;
        public int buffSubType;
        public float[] buffPower;
        public int buffCategory;
        public float buffDuration;
        public int buffDispellType = 1;
        public int applyType;
        public int buffMaxStack;
        public int stackDecrease;
        public string buffIconPath;
        public ValueType valueType;
        public bool showIcon;
        public int applyStrategy;

        public BuffDataType(SubBuffType type)
        {
            if (BuffDatabase.DataLoad.TryGetSubBuffIndex(type, out index))
            {
                buffMainType = index / 100;
                buffSubType = index % 100;
            }
        }
    }

    [Serializable]
    public class BuffGroupDataType : HasChance
    {
        public int index;
        public int buffGroup;
        public int buffIndex;
        public int buffName;
        public int buffDesc;
        public bool showIcon;
        public string iconPath;
        public int chance { get; set; }
    }

    [Serializable]
    public class SubBuffTypeDataType
    {
        public int index;
        public SubBuffType typeName;
    }
}