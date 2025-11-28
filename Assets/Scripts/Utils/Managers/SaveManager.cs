using System;
using System.Collections.Generic;
using Save.Schema;

namespace Managers
{
    public class SaveManager
    {
        public enum SaveType
        {
            Persistent,
            Slot
        }

        private readonly Dictionary<SaveType, DataSchema> _saveData = new()
        {
            { SaveType.Persistent, new DataSchema() },
            { SaveType.Slot, new DataSchema() }
        };

        private SlotData _currentSlotData;

        public SaveManager()
        {
            SetPersistentData();
        }

        public SlotData currentSlotData
        {
            get
            {
                if (_currentSlotData == null) SetCurrentSlotData(new SlotData("0"));
                return _currentSlotData;
            }
        }

        public static string GetSlotDataKey(string id)
        {
            return $"Slot{id}";
        }

        public void SetCurrentSlotData(SlotData slotData)
        {
            _currentSlotData = slotData;
            SetSlotData(currentSlotData.slotId);
        }

        public void SetPersistentData()
        {
            _saveData[SaveType.Persistent]
                .AddData(PersistentDataKeys.GetKey(PersistentDataKeys.DataTypes.Setting), new SettingData());
            _saveData[SaveType.Persistent]
                .AddData(PersistentDataKeys.GetKey(PersistentDataKeys.DataTypes.GameData), new GameSaveData());
        }

        public void SetSlotData(string slotId)
        {
            _saveData[SaveType.Slot].AddData(SlotDataKeys.GetKey(SlotDataKeys.DataTypes.SlotInfo, slotId),
                new SlotInfoSaveData());
        }

        public ISaveData GetData(PersistentDataKeys.DataTypes type)
        {
            return _saveData[SaveType.Persistent].Datas[PersistentDataKeys.GetKey(type)];
        }

        public ISaveData GetData(SlotDataKeys.DataTypes type, string slotId)
        {
            return _saveData[SaveType.Slot].Datas[SlotDataKeys.GetKey(type, slotId)];
        }

        public void SaveData(SaveType saveType)
        {
            if (_saveData.TryGetValue(saveType, out var value)) value.SaveAll();
        }

        public void SaveData(string key, ISaveData data)
        {
            if (data == null) return;

            data.BeforeSave();
            ES3.Save(key, data);
        }

        public void LoadPersistentData()
        {
            if (_saveData.TryGetValue(SaveType.Persistent, out var value)) value.LoadAll();
        }

        public void LoadSlotData(string slotId)
        {
            SetCurrentSlotData(new SlotData(slotId));
            _saveData[SaveType.Slot].LoadAll();
        }

        public void ResetSlotData()
        {
            _saveData[SaveType.Slot].ResetAll();
        }

        public void LoadExceptSlot()
        {
            LoadPersistentData();
            // 해당 데이터 이용해서 모두 초기화띠
        }


        public static void ClearDataFiles()
        {
            var slotData = DataAccess.GameData.Data.SlotDatas;


            foreach (SlotDataKeys.DataTypes dataType in Enum.GetValues(typeof(SlotDataKeys.DataTypes)))
            {
                if (ES3.KeyExists(SlotDataKeys.GetKey(dataType, "0")))
                    ES3.DeleteKey(SlotDataKeys.GetKey(dataType, "0"));

                for (var i = 0; i < DataAccess.GameData.Data.SlotDatas.Count; i++)
                    if (ES3.KeyExists(SlotDataKeys.GetKey(dataType, slotData[i].SlotId)))
                        ES3.DeleteKey(SlotDataKeys.GetKey(dataType, slotData[i].SlotId));
            }

            foreach (PersistentDataKeys.DataTypes dataType in Enum.GetValues(typeof(PersistentDataKeys.DataTypes)))
                if (ES3.KeyExists(PersistentDataKeys.GetKey(dataType)))
                    ES3.DeleteKey(PersistentDataKeys.GetKey(dataType));
        }
    }
}