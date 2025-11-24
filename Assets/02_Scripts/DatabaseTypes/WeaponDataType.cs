using Apis;

namespace Apis.DataType
{
    [System.Serializable]
    public class WeaponDataType
    {
        // public int index;
        public int weaponId;
        public int weaponNameString;
        public int description;
        public Weapon.WeaponGrade weaponRarity;
        public float bodyFactor;
        public float spiritFactor;
        public float finesseFactor;
        public float attackPower;
        public int baseGroggyPower;
        public string iconPath;
        public Define.WeaponType weaponType;
        // public int price;
        public bool unlock;
    }
    
    [System.Serializable]
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
    
    [System.Serializable]
    public class MotionDataType
    {
        public int index;
        public string motionName;
    }
}