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
            Slot,
        }

        public static string GetSlotDataKey(string id) => $"Slot{id}";

        private readonly Dictionary<SaveType, DataSchema> _saveData = new();

        public static string PersistentFileName => "persistent.es3";
        public static string SlotFileName(string slotId) => $"slot_{slotId}.es3";

        SlotData _currentSlotData;

        public SlotData currentSlotData
        {
            get
            {
                if (_currentSlotData == null)
                {
                    SetCurrentSlotData(new SlotData("0"));
                }

                return _currentSlotData;
            }
        }

        public void SetCurrentSlotData(SlotData slotData)
        {
            _currentSlotData = slotData;
            SetSlotData(currentSlotData.slotId);
        }

        public SaveManager()
        {
            SetPersistentData();
        }

        public void SetPersistentData()
        {
            _saveData.TryAdd(SaveType.Persistent, new PersistentDataSchema());
            _saveData[SaveType.Persistent] ??= new PersistentDataSchema();

            _saveData[SaveType.Persistent]
                .AddData(PersistentDataKeys.GetKey(PersistentDataKeys.DataTypes.Setting), new SettingData());
            _saveData[SaveType.Persistent]
                .AddData(PersistentDataKeys.GetKey(PersistentDataKeys.DataTypes.GameData), new GameSaveData());
        }

        public void SetSlotData(string slotId)
        {
            _saveData.TryAdd(SaveType.Slot, new SlotDataSchema());
            _saveData[SaveType.Slot] ??= new SlotDataSchema();

            _saveData[SaveType.Slot].AddData(SlotDataKeys.GetKey(SlotDataKeys.DataTypes.SlotInfo, slotId),
                new SlotInfoSaveData());

            // 플레이어 관련 데이터 로딩이 제일 마지막에 되어야해서 TempSaveData는 맨 마지막에 와야함
            // TempSaveData 내부에 플레이어 관련 데이터와 플레이어 생성 로직이 작성되어있음
            _saveData[SaveType.Slot].AddData(SlotDataKeys.GetKey(SlotDataKeys.DataTypes.Temp, slotId),
                new TempSaveData());

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
            if (_saveData.TryGetValue(saveType, out var value))
            {
                value.SaveAll();
            }
        }

        public void SaveData(SaveType saveType, string key)
        {
            if (_saveData.TryGetValue(saveType, out var value))
                value.Save(key);
        }

        public void LoadPersistentData()
        {
            if (_saveData.TryGetValue(SaveType.Persistent, out var value))
            {
                value.LoadAll();
            }
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

        public void DeleteData(SaveType saveType, string key)
        {
            if (_saveData.TryGetValue(saveType, out var value))
            {
                value.DeleteData(key);
            }
        }

        public static void ClearDataFiles()
        {
            var slotData = DataAccess.GameData.Data.SlotDatas;

            ES3.DeleteFile(SlotFileName("0"));
            for (int i = 0; i < DataAccess.GameData.Data.SlotDatas.Count; i++)
            {
                ES3.DeleteFile(SlotFileName(slotData[i].SlotId));
            }

            ES3.DeleteFile(PersistentFileName);
        }
    }
}