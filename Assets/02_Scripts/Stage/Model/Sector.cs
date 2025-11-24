using System;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis;
using Apis.CommonMonster2;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using System.IO;
using Default;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

public class Sector : SerializedMonoBehaviour
{
    public int sectorID;
    
    
    private List<Monster> _monsters = new();
    [ReadOnly] public Dictionary<SectorObject,Guid> sectorObjects = new();

    public SectorManager.SectorType sectorType = SectorManager.SectorType.MainWorld;

    public static Action<Sector> WhenSectorEnter;
    public static Action<Sector> WhenSectorExit;

    public UnityEvent OnSectorEnter;
    public UnityEvent OnSectorExit;

    static int currentSector;

    private bool isEntered;
    private void Awake()
    {
        Load();
        isEntered = false;
    }

    [ReadOnly] public SectorObjectsData ObjectData;

#if UNITY_EDITOR

    [Button("섹터 데이터 생성",50)]
    void CreateSectorData()
    {
        if (ObjectData == null)
        {
            ObjectData = ScriptableObject.CreateInstance(typeof(SectorObjectsData)) as SectorObjectsData;
        }
        string path = "Assets/06_ScriptableObjects/Datas/Sector/";
        if (!File.Exists(path + sectorID + ".asset"))
            AssetDatabase.CreateAsset(ObjectData, path + sectorID + ".asset");
        
        AssetDatabase.SaveAssets();
    }

    [Button("섹터 데이터 삭제",50)]
    void RemoveSectorData()
    {
        AssetDatabase.DeleteAsset($"Assets/06_ScriptableObjects/Datas/Sector/{sectorID}.asset");
        ObjectData = null;
        AssetDatabase.SaveAssets();
    }
    [Button("몬스터 데이터 등록",50)]
    void ChangeMonsters()
    {
        CreateSectorData();
        var monsters = GetComponentsInChildren<Monster>(true);

        ObjectData.monsterDatas ??= new();
        foreach (var monster in monsters)
        {
            string guid = string.Empty;
            Transform parent = monster.transform.parent;
            if (parent != null)
            {
                if (!parent.gameObject.TryGetComponent(out GUIDHandler handler))
                {
                    handler = parent.gameObject.AddComponent<GUIDHandler>();
                    handler.guid = Guid.NewGuid().ToString();
                }

                guid = handler.guid;
                EditorUtility.SetDirty(handler);
            }
            SectorObjectsData.MonsterData data = new SectorObjectsData.MonsterData(monster.transform.position,
                guid, Guid.NewGuid().ToString(), monster.monsterId, monster.transform.localScale);
            ObjectData.monsterDatas.Add(data);
            DestroyImmediate(monster.gameObject);
        }
        
        EditorUtility.SetDirty(ObjectData);
        AssetDatabase.SaveAssets();
    }
    [Button("몬스터 데이터 삭제 및 몬스터 재생성",50)]
    void CreateMonsters()
    {
        if (ObjectData == null) return;
        
        foreach (var monster in ObjectData.monsterDatas)
        {
            GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(
                ResourceUtil.Load<CommonMonster2>($"Prefabs/Components/MonsterComponent/{monster.monsterID}")
                    .gameObject);
            var handlers = gameObject.GetComponentsInChildren<GUIDHandler>(true);
            GUIDHandler parent = null;
            foreach (var guidHandler in handlers)
            {
                if (guidHandler.guid == monster.parentGuid)
                {
                    parent = guidHandler;
                    break;
                }
            }

            if (parent != null)
            {
                newInstance.transform.SetParent(parent.transform);
            }
            CommonMonster2 cm = newInstance.GetComponent<CommonMonster2>();
            cm.isAlreadyCreated = true;
            cm.transform.position = monster.position;
            cm.transform.localScale = monster.scale;
            EditorUtility.SetDirty(cm);
        }
        ObjectData.monsterDatas.Clear();
        EditorUtility.SetDirty(ObjectData);
        AssetDatabase.SaveAssets();
    }

    [Button("오브젝트 데이터 등록",50)]
    void RegisterObjects()
    {
        sectorObjects ??= new();
        var objects = GetComponentsInChildren<SectorObject>();
        foreach (var obj in objects)
        {
            if (!sectorObjects.ContainsKey(obj))
            {
                Guid guid = Guid.NewGuid();
                sectorObjects.Add(obj,guid);
                obj.guid = guid;
            }
        }
    }

    [Button("오브젝트 데이터 삭제", 50)]
    void RemoveObjects()
    {
        sectorObjects?.ForEach(x =>
        {
            x.Key.guid = Guid.Empty;
        });
        sectorObjects?.Clear();
    }
    private void OnDrawGizmos()
    {
        if (ObjectData == null) return;
        if (ObjectData.monsterDatas == null) return;
        
        foreach (var monster in ObjectData.monsterDatas)
        {
            Handles.Label(monster.position, monster.monsterID.ToString());
        }
    }


#endif

    public void Load()
    {
        GameManager.SectorMag.AddSector(sectorID, this);
    }
    public void Enter()
    {
        if (isEntered) return;
        isEntered = true;
        LoadMonsters();
        ResetMonsters();
        WhenSectorEnter?.Invoke(this);
        OnSectorEnter?.Invoke();
    }

    public void Exit()
    {
        if (!isEntered) return;
        isEntered = false;
        RemoveMonsters();
        WhenSectorExit?.Invoke(this);
        OnSectorExit?.Invoke();
    }

    public void LoadMonsters()
    {
        if (ObjectData == null) return;
        
        foreach (var monsterData in ObjectData.monsterDatas)
        {
            if (GameManager.Save.currentSlotData != null &&
                GameManager.Save.currentSlotData.SectorSaveData.monsterKilled.Contains(monsterData.guid))
                continue;

            if (_monsters.Find(x => x.Guid != Guid.Empty && x.Guid == Guid.Parse(monsterData.guid)) != null)
            {
                continue;
            }
            Monster monster = GameManager.Factory.Get<Monster>(FactoryManager.FactoryType.Monster, $"Prefabs/Components/MonsterComponent/{monsterData.monsterID}",
                monsterData.position);
            var handlers = gameObject.GetComponentsInChildren<GUIDHandler>(true);
            GUIDHandler parent = null;
            foreach (var guidHandler in handlers)
            {
                if (guidHandler.guid == monsterData.parentGuid)
                {
                    parent = guidHandler;
                    break;
                }
            }

            if (parent != null)
            {
                monster.transform.SetParent(parent.transform);
            }
            monster.transform.localScale = monsterData.scale;
            _monsters.Add(monster);
            monster.Guid = Guid.Parse(monsterData.guid);
            monster.AddEvent(EventType.OnDeath,SetMonsterDeath);
        }
    }
    public void ResetMonsters()
    {
        if (ObjectData == null) return;
        
        _monsters.ForEach(monster =>
        {
            monster.OnGet();
            SectorObjectsData.MonsterData data = ObjectData.monsterDatas.Find(x => Guid.Parse(x.guid) == monster.Guid);
            if (monster.Guid != Guid.Empty && Guid.Parse(data.guid) != monster.Guid) return;
            monster.transform.position = data.position;
        });
    }

    void RemoveMonsters()
    {
        _monsters.ForEach(monster =>
        {
            monster.ReturnToPool();
        });
        _monsters.Clear();
    }
    void SetMonsterDeath(EventParameters param)
    {
        if (param.user is Monster monster)
        {
            if (GameManager.Save.currentSlotData != null)
            {
                GameManager.Save.currentSlotData.SectorSaveData.monsterKilled.Add(monster.Guid.ToString());
            }
            monster.Guid = Guid.Empty;
            monster.RemoveEvent(EventType.OnDeath,SetMonsterDeath);
            _monsters.Remove(monster);
        }
    }
    public void UnLoad()
    {
        _monsters.ForEach(x =>
        {
            if (x != null)
            {
                x.Guid = Guid.Empty;
                GameManager.Factory.Return(x.gameObject);
            }
        });
    }
}