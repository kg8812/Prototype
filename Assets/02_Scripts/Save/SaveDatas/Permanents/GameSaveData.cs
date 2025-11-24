using System.Collections.Generic;
using Managers;

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
            SlotDatas = new();
        }

        public void BeforeSave()
        {
        }
        
    }
}