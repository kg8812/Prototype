using Managers;

namespace Save.Schema
{
    public static class PersistentDataKeys
    {
        public enum DataTypes
        {
            Setting,CollectionOpened,CutSceneOpened,TipOpened,GameData,
        }
        
        const string GameDataKey ="GameDataKey";
        const string SettingKey = "SettingKey";
        const string CollectionKey = "CollectionOpenedKey";
        const string CutSceneKey = "CutSceneOpenedKey";
        const string TipKey = "TipKey";
        public static string GetKey(DataTypes type)
        {
            return type switch
            {
                DataTypes.Setting => SettingKey,
                DataTypes.CollectionOpened => CollectionKey,
                DataTypes.CutSceneOpened => CutSceneKey,
                DataTypes.TipOpened => TipKey,
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
            Growth,
            NPC,
            Temp,
            Lobby,
            Sector,
            Task,
            Map
        }
        
        const string SlotInfoKey ="SlotInfoKey";
        const string GrowthKey = "GrowthKey";
        const string NPCKey = "NPCKey";
        const string Temp = "TempKey";
        const string Lobby = "LobbyKey";
        const string Task = "TaskKey";
        const string Map = "MapKey";
        private const string Sector = "SectorKey";
        
        public static string GetKey(DataTypes type,string slotIndex)
        {
            
            string slotId = SaveManager.GetSlotDataKey(slotIndex);
            return slotId + type switch
            {
                DataTypes.SlotInfo => SlotInfoKey,
                DataTypes.Growth => GrowthKey,
                DataTypes.NPC => NPCKey,
                DataTypes.Temp => Temp,
                DataTypes.Lobby => Lobby,
                DataTypes.Task => Task,
                DataTypes.Sector => Sector,
                DataTypes.Map => Map,
                _ => ""
            };
        }
    }
}