using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis;
using Apis.Components;
using Default;
using Save.Schema;
using Sirenix.Utilities;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SectorManager
{
    public enum LoadType
    {
        Load,UnLoad
    }
    
    public struct SectorLoadData
    {
        public int sectorId;
        public LoadType loadType;
        public bool activate;
    }
    private readonly Queue<SectorLoadData> sectorQueue = new Queue<SectorLoadData>();

    private bool isLoading = false;

    public readonly Dictionary<int,Sector> loadedSectors = new();
    public readonly HashSet<int> enteredSectors = new();

    private HashSet<Projectile> _activatedProjectiles = new(); 
    public HashSet<Projectile> ActivatedProjectiles => _activatedProjectiles ??= new(); 
    public bool IsFirstSector { get; set; }

    private SectorBaseData _sectorBaseData;
    
    public SectorBaseData SectorBaseData
    {
        get
        {
            if (_sectorBaseData == null)
            {
                _sectorBaseData = ResourceUtil.Load<SectorBaseData>("SectorBaseData");
            }

            return _sectorBaseData;
        }
    }

    public enum SectorType
    {
        MainWorld
    }

    private Dictionary<SectorType, int> _currentSectors;
    public Dictionary<SectorType, int> CurrentSectors => 
        _currentSectors ??= new Dictionary<SectorType, int> { { SectorType.MainWorld, SectorBaseData.startIndex } };

    private Dictionary<int, int> _loadSectorCounts;
    private Dictionary<int, int> LoadSectorCounts => _loadSectorCounts;

    private Dictionary<int,TA_EnterSector> _enteredSectorTriggers = new();

    public Dictionary<int,TA_EnterSector> EnteredSectorTriggers
    {
        get
        {
            _enteredSectorTriggers ??= new();
            _enteredSectorTriggers = _enteredSectorTriggers.Where(x => x.Value.trigger != null)
                .ToDictionary(x => x.Key, x => x.Value);

            return _enteredSectorTriggers;
        }
    }

    private int _curEnteredSector;

    public int CurEnteredSector
    {
        get => _curEnteredSector;
        set
        {
            _curEnteredSector = value;
            CurEnteredSectorChanged?.Invoke(value);
        }
    }
    public Action<int> CurEnteredSectorChanged;

    public SectorManager()
    {
        CurEnteredSectorChanged += _ =>
        {
            var temp = ActivatedProjectiles.ToList();
            foreach (var activatedProjectile in temp)
            {
                activatedProjectile?.Destroy();
            }

            ActivatedProjectiles.Clear();
        };
    }
    public void EnterSectorTrigger(int index, TA_EnterSector sectorTrigger)
    {
        EnteredSectorTriggers.TryAdd(index,sectorTrigger);
        CurEnteredSector = index;
    }
    public void ExitSectorTrigger(int index)
    {
        EnteredSectorTriggers.Remove(index);
    }
    public void SetLastShelter()
    {
        int nameId = Calc.ConcatInts(StrUtil.MapBoxNameCategory, Map.instance.CurMapBox);
        string sceneName = SceneManager.GetActiveScene().name;
        Vector2 position = GameManager.instance.ControllingEntity.transform.position;

        GameManager.Save.currentSlotData.SectorSaveData.lastSavedPosition = new SectorSaveData.ShelterData
        {
            sceneName = sceneName,
            savedPosition = position
        };
        GameManager.Save.currentSlotData.InfoData.PlaceNameId = nameId;
    }

    public void MoveToLastShelter(bool loadScene)
    {
        var savedPosition = GameManager.Save.currentSlotData.SectorSaveData.lastSavedPosition;
        Vector2 position = savedPosition.savedPosition;

        ResetSectors();
        if (loadScene)
        {
            string sceneName =  savedPosition.sceneName;

            // sceneName을 null과 비교해서 데이터 등록이 된건지 판단함.
            // 데이터 등록이 안되었으면 spawnPos를 등록하면 안됨.
            if (string.IsNullOrEmpty(savedPosition.sceneName))
            {
                sceneName = SceneManager.GetActiveScene().name;
            }
            else
            {
                SpawnPoint.spawnPos.Enqueue(position);
            }

            GameManager.Scene.SceneLoad(sceneName);
        }
        else
        {
            if (GameManager.instance.Player != null)
            {
                GameManager.instance.ControllingEntity.transform.position = position;
                GameManager.instance.Player.gameObject.SetActive(true);
            }
        }
    }
    
    public HashSet<int> GetRequiredSectorsBasedOnEnteredTriggers()
    {
        HashSet<int> required = new HashSet<int>();
        // EnteredSectorTriggers는 항상 최신 상태를 유지해야 함
        foreach (var triggerPair in EnteredSectorTriggers) // Manager의 멤버 사용
        {
            // Value (TA_EnterSector instance) 및 trigger 객체 유효성 검사 추가
            if (triggerPair.Value?.trigger != null)
            {
                required.Add(triggerPair.Value.sectorId);
                if (triggerPair.Value.injectionIds != null)
                {
                    foreach (var id in triggerPair.Value.injectionIds)
                    {
                        required.Add(id);
                    }
                }
            }
        }
        return required;
    }
    public void AddSector(int index, Sector sector)
    {
        loadedSectors.TryAdd(index, sector);
    }
    
    public void LoadSector(int sectorIndex, UnityAction afterLoad, bool activate = true)
    {
        string sceneName = $"Sector{sectorIndex}";

        int count = SceneManager.sceneCount;
        for (int i = 0; i < count; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName) return;
        }

        SectorLoadData data = new()
        {
            sectorId = sectorIndex,
            loadType = LoadType.Load,
            activate = activate,
        };
        sectorQueue.Enqueue(data);

        if (!isLoading)
        {
            FadeManager.instance.LoadingQueue.Enqueue(GameManager.instance.StartCoroutine(LoadSectorCoroutine(afterLoad)));
        }
    }

    public void UnloadSector(int sectorIndex,UnityAction afterLoad)
    {
        SectorLoadData data = new()
        {
            sectorId = sectorIndex,
            loadType = LoadType.UnLoad,
            activate = false,
        };
        if (loadedSectors.TryGetValue(sectorIndex, out Sector sector))
        {
            sector.UnLoad();
            loadedSectors.Remove(sectorIndex);
        }
        sectorQueue.Enqueue(data);

        if (!isLoading)
        {
           FadeManager.instance.LoadingQueue.Enqueue(GameManager.instance.StartCoroutine(LoadSectorCoroutine(afterLoad)));
        }
    }

    public void UnLoadAllSectors(Scene s1,Scene s2)
    {
        loadedSectors.Clear();
        enteredSectors.Clear();
    }
    IEnumerator LoadSectorCoroutine(UnityAction afterLoad)
    {
        if (isLoading) yield break;
        isLoading = true;
        while (sectorQueue.Count > 0)
        {
            SectorLoadData data = sectorQueue.Dequeue();
            string sceneName = "Sector" + data.sectorId;
            AsyncOperation asyncOperation = data.loadType == LoadType.Load
                ? SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive)
                : SceneManager.UnloadSceneAsync(sceneName);

            if (data.loadType == LoadType.UnLoad && loadedSectors.ContainsKey(data.sectorId))
            {
                loadedSectors[data.sectorId].UnLoad();
                loadedSectors.Remove(data.sectorId);
                enteredSectors.Remove(data.sectorId);
            }
            if (asyncOperation != null)
            {
                asyncOperation.allowSceneActivation = data.activate;
            }
            
            while (asyncOperation is { isDone: false })
            {
                yield return new WaitForSecondsRealtime(0.05f);
            }
        }

        isLoading = false;
        afterLoad?.Invoke();
    }

    public void ResetSectors()
    {
        if (GameManager.Save.currentSlotData != null)
        {
            GameManager.Save.currentSlotData.SectorSaveData.monsterKilled.Clear();
        }

        loadedSectors.ForEach(x => x.Value.LoadMonsters());
    }

}