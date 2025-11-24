using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Apis.DataType;
using System;
using System.Reflection;
using System.Linq;
using Save.Schema;
using TMPro;
using UnityEngine.Events;

namespace Apis
{
    // string table 제외 (string table은 모든 곳에서 너무 많이 씀)
    public enum DataTableType
    {
        // Weapon,
        Drop, DropGroup,
        
        Sector, Floor, Map,
        
        Buff, SubBuffOption, BuffGroup, SubBuffType,
        Weapon, Motion,MotionGroup,
        Accessory,
        ActiveSkillItem,
        Config,
        
        Monster, SkillTree,Shop,ShopList,
        
        Tmi, Tip, Script, Note, InGameCutScene,CutScene,
        
        Convenience,Growth,Level,
    }
}

public class DataBaseInit
{

}

namespace Apis.Managers
{
    public class DatabaseManager
    {
        private Dictionary<DataTableType, object> database = new Dictionary<DataTableType, object>();

        public static string GetStringFromJson(string path)
        {
            string devJsonFilePath = "Database"; // 해당 위치로 데이터베이스 json 저장
            // string distJsonFilePath = "StreamingAssets";

            string json = "";
            TextAsset jsonFile = Resources.Load<TextAsset>(Path.Combine(devJsonFilePath, path));
            if (jsonFile != null)
            {
                json = jsonFile.text;
            }
            

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
            string json = GetStringFromJson(dataTableName);
            return JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
        }

        bool isInit = false;
        public void Init()
        {
            if (isInit) return;
            
            foreach (DataTableType dtt in Enum.GetValues(typeof(DataTableType)))
            {
                Type dataType = Type.GetType($"Apis.DataType.{Enum.GetName(typeof(DataTableType), dtt)}DataType");
                string tableName = $"{Enum.GetName(typeof(DataTableType), dtt)}Table";              
                MethodInfo method = this.GetType().GetMethod("GetDataTableInJson").MakeGenericMethod(dataType);
                object[] parameters = { tableName };
                var datas = method.Invoke(this, parameters);
                database.Add(dtt, datas);
            }
            Load();
            

            isInit = true;
        }

        public void Load()
        {
            
            
            if(isInit) return;
            
            // 랭귀지쪽에서 먼저 처리 함.
            new LanguageManager().ProcessDataLoad();
            var types = typeof(Database).Assembly.GetTypes().Where(v => v.IsSubclassOf(typeof(Database)));           

            foreach (var type in types)
            {
                if(type != typeof(LanguageManager))
                    (Activator.CreateInstance(type) as Database)?.ProcessDataLoad();
            }
        }
        public Dictionary<string, T> GetDataTable<T>(DataTableType dataTableType)
        {
            if (database.TryGetValue(dataTableType, out object obj))
            {
                return obj as Dictionary<string, T>;
            }
            else
            {
                throw new Exception($"There is no database {dataTableType.ToString()}");
            }
        }

        
    }
}