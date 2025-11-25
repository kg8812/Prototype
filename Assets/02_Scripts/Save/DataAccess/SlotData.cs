using System.Collections;
using System.Collections.Generic;
using Managers;
using Save.Schema;
using UnityEngine;

namespace Save.Schema
{
    public abstract class SlotSaveData : ISaveData
    {
        public string SlotId;

        public void BeforeSave()
        {
            if (SlotId == GameManager.Save.currentSlotData.slotId)
            {
                OnBeforeSave();
            }
        }

        public void OnLoaded()
        {
            // Debug.Log($"slot data on loaded {SlotId == GameManager.Save.currentSlotData.slotId}");
            if (SlotId == GameManager.Save.currentSlotData.slotId)
            {
                BeforeLoaded();
            }
        }

        public void Initialize()
        {
            if (SlotId == GameManager.Save.currentSlotData.slotId)
            {
                OnReset();
            }
        }

        protected abstract void OnBeforeSave();
        protected abstract void BeforeLoaded();
        protected abstract void OnReset();
    }

    public class SlotData
    {
        public readonly string slotId;

        public SlotData(string slotID)
        {
            GameManager.Save.SetSlotData(slotID);
            slotId = slotID;
            InfoData.SlotId = slotID;
        }

        public SlotInfoSaveData InfoData =>
            GameManager.Save.GetData(SlotDataKeys.DataTypes.SlotInfo, slotId) as SlotInfoSaveData;
        
        public void UpdateSlotDataToGameData()
        {
            int index = -1;
            var slotDatas = DataAccess.GameData.Data.SlotDatas;

            for (int i = 0; i < slotDatas.Count; i++)
            {
                if (slotDatas[i].SlotId == slotId)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                DataAccess.GameData.Data.SlotDatas.Add(InfoData);
            }
            else
            {
                DataAccess.GameData.Data.SlotDatas[index] = InfoData;
            }

            DataAccess.GameData.Save();
        }
    }
}