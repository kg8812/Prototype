using System.Collections.Generic;

namespace Save.Schema
{
    public class GameSaveData : ISaveData
    {
        public bool IsFirstGame = true;

        public List<SlotInfoSaveData> SlotDatas = new();

        public void OnLoaded()
        {
        }

        public void Initialize()
        {
            SlotDatas = new List<SlotInfoSaveData>();
        }

        public void BeforeSave()
        {
        }
    }
}