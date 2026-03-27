using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Apis
{
    // string table 제외 (string table은 모든 곳에서 너무 많이 씀)
    public enum DataTableType
    {
        Buff,
        SubBuffOption,
        BuffGroup,
        SubBuffType,
        Config,

        Monster,
        SkillTree,
        Level
    }
}

public class DataBaseInit
{
}

namespace Apis.Managers
{
    public class DatabaseManager
    {
        private readonly Dictionary<DataTableType, object> database = new();

        private bool isInit;

        public static string GetStringFromJson(string path)
        {
            var devJsonFilePath = "Database"; // 해당 위치로 데이터베이스 json 저장
            // string distJsonFilePath = "StreamingAssets";

            var json = "";
            var jsonFile = Resources.Load<TextAsset>(Path.Combine(devJsonFilePath, path));
            if (jsonFile != null) json = jsonFile.text;


            // string realPath = Path.Combine(Application.persistentDataPath, distJsonFilePath, path);
            // if(File.Exists(realPath)){
            //     json = File.ReadAllText(realPath);          
            // }else{
            //     throw new System.Exception("File not found at: " + realPath);
            // }
            return json;
        }

        public Dictionary<string, T> GetDataTableInJson<T>(string dataTableName)
        {
            var json = GetStringFromJson(dataTableName);
            return JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
        }

        public void Init()
        {
            if (isInit) return;

            foreach (DataTableType dtt in Enum.GetValues(typeof(DataTableType)))
            {
                var dataType = Type.GetType($"Apis.DataType.{Enum.GetName(typeof(DataTableType), dtt)}DataType");
                var tableName = $"{Enum.GetName(typeof(DataTableType), dtt)}Table";
                var method = GetType().GetMethod("GetDataTableInJson").MakeGenericMethod(dataType);
                object[] parameters = { tableName };
                var datas = method.Invoke(this, parameters);
                database.Add(dtt, datas);
            }

            Load();


            isInit = true;
        }

        public void Load()
        {
            if (isInit) return;

            // 랭귀지쪽에서 먼저 처리 함.
            var types = typeof(Database).Assembly.GetTypes().Where(v => v.IsSubclassOf(typeof(Database)));

            foreach (var type in types) (Activator.CreateInstance(type) as Database)?.ProcessDataLoad();
        }

        public Dictionary<string, T> GetDataTable<T>(DataTableType dataTableType)
        {
            if (database.TryGetValue(dataTableType, out var obj)) return obj as Dictionary<string, T>;

            throw new Exception($"There is no database {dataTableType.ToString()}");
        }
    }
}