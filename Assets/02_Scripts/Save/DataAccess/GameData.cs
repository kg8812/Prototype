using System.Collections;
using System.Collections.Generic;
using Save.Schema;
using UnityEngine;

namespace Save.Schema
{
    public class GameData
    {
        public GameSaveData Data => GameManager.Save.GetData(PersistentDataKeys.DataTypes.GameData) as GameSaveData;

        public void Save()
        {
            GameManager.Save.SaveData(PersistentDataKeys.GetKey(PersistentDataKeys.DataTypes.GameData),Data);
        }
    }
}