using System.Collections.Generic;
using Managers;
using Save.Schema;

public enum LanguageType
{
    Korean,
    English
}

public class Settings
{
    public SettingData Data
    {
        get
        {
            var data = GameManager.Save.GetData(PersistentDataKeys.DataTypes.Setting) as SettingData;
            return data;
        }
    }

    public void Save()
    {
        GameManager.Save.SaveData(PersistentDataKeys.GetKey(PersistentDataKeys.DataTypes.Setting),Data);
    }
}
