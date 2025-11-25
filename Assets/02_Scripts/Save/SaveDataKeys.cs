using Managers;

namespace Save.Schema
{
    public static class PersistentDataKeys
    {
        public enum DataTypes
        {
            Setting,GameData,
        }
        
        const string GameDataKey ="GameDataKey";
        const string SettingKey = "SettingKey";
        public static string GetKey(DataTypes type)
        {
            return type switch
            {
                DataTypes.Setting => SettingKey,
                DataTypes.GameData => GameDataKey,
                _ => ""
            };
        }
    }

    public static class SlotDataKeys
    {
        public enum DataTypes
        {
            SlotInfo,
        }
        
        const string SlotInfoKey ="SlotInfoKey";
        
        public static string GetKey(DataTypes type,string slotIndex)
        {
            
            string slotId = SaveManager.GetSlotDataKey(slotIndex);
            return slotId + type switch
            {
                DataTypes.SlotInfo => SlotInfoKey,
                _ => ""
            };
        }
    }
}