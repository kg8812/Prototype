using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apis;
using Managers;
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Save.Schema
{
    public abstract class DataSchema
    {
        private Dictionary<string, ISaveData> _datas;
        public Dictionary<string, ISaveData> Datas => _datas ??= new();

        protected abstract ES3File SaveFile { get; }
        protected abstract string FileName { get; }

        public void Save(string key)
        {
            if (Datas.TryGetValue(key, out var value))
            {
                if (value != null)
                {
                    value.BeforeSave();
                    SaveFile.Save(key,value);
                    SaveFile.Sync();
                }
            }
        }
        public void SaveAll()
        {
            Datas?.ForEach(x =>
            {
                if (Datas[x.Key] == null) return;
                Datas[x.Key].BeforeSave();
                SaveFile.Save(x.Key, x.Value);
            });
            SaveFile.Sync();
        }

        public void LoadAll()
        {
            var keys = Datas.Keys.ToList();
            foreach (var x in keys)
            {
                if (SaveFile.KeyExists(x))
                {
                    try
                    {
                        var existing = Datas[x];
                        SaveFile.LoadInto(x, existing);
                        existing.OnLoaded();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        SystemManager.SystemAlert("date load error!\nConfirm to data clear and restart the game.", () =>
                        {
                            Initializer.DataClear();
#if UNITY_EDITOR
                            EditorApplication.isPlaying = false;
#else
                            string exePath = Process.GetCurrentProcess().MainModule.FileName;
                            Process.Start(exePath);
                            Application.Quit();
#endif
                        });
                        throw;
                    }
                    
                }
            }
        }

        public void ResetAll()
        {
            Datas?.ForEach(x =>
            {
                if (Datas[x.Key] == null) return;
                Datas[x.Key].Initialize();
            });
        }

        public void AddData(string key,ISaveData data)
        {
            Datas?.TryAdd(key, data);
        }

        public void DeleteData(string key)
        {
            if (SaveFile.KeyExists(key))
            {
                SaveFile.DeleteKey(key);
            }
        }
    }

    public class SlotDataSchema : DataSchema
    {
        readonly Dictionary<string, ES3File> _slotSaveFiles = new();
        protected override ES3File SaveFile
        {
            get
            {
                var slotId = GameManager.Save.currentSlotData.slotId;
                if (!_slotSaveFiles.ContainsKey(slotId))
                {
                    _slotSaveFiles.Add(slotId, new ES3File(SaveManager.SlotFileName(slotId)));
                }
                
                _slotSaveFiles[slotId] ??= new ES3File(SaveManager.SlotFileName(slotId));
                
                return  _slotSaveFiles[slotId];
            }
        }

        protected override string FileName
        {
            get
            {
                var slotId = GameManager.Save.currentSlotData.slotId;
                return SaveManager.SlotFileName(slotId);
            }
        }
    }

    public class PersistentDataSchema : DataSchema
    {
        private ES3File _saveFile;

        protected override ES3File SaveFile => _saveFile ??= new ES3File(SaveManager.PersistentFileName);

        protected override string FileName => SaveManager.PersistentFileName;
    }
    public interface ISaveData
    {
        public void BeforeSave(); // 세이브 전 호출코드, 세이브하기전에 이 클래스에 현재 게임 내 데이터 값을 가져와야함. ex) 현재 재화값을 데이터로 가져옴
        public void OnLoaded(); // 로드시 호출코드, 이 클래스에 저장된 데이터들을 게임에 적용시킴.
        public void Initialize(); // 이 함수는 이 클래스의 데이터를 초기화하는용이 아니라, 현재 이 세이브 데이터에 맞는 게임 내 데이터를 초기화하는용임.
                                 //  예시) 재화관련 세이브를 관리하는 클래스일 시, 현재 게임내의 재화를 0으로 초기화. 새로운 슬롯을 만들 때 초기화하기 위해 필요한 함수
    }
}