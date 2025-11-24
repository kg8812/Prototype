using System;
using System.Collections.Generic;
using Apis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SectorObjectsData : ScriptableObject
{
    [Serializable]
    public struct MonsterData
    {
        public Vector2 position;
        public string parentGuid;
        public string guid;
        public int monsterID;
        public Vector3 scale;
        
        public MonsterData(Vector2 position, string parentGuid,string guid,int monsterID,Vector3 scale)
        {
            this.position = position;
            this.parentGuid = parentGuid;
            this.guid = guid;
            this.monsterID = monsterID;
            this.scale = scale;
        }
    }
    [ReadOnly] public List<MonsterData> monsterDatas;
}
