using System;
using System.Collections.Generic;
using System.Linq;
using Default;
using Save.Schema;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Apis.Components
{
    public enum TriggerStrategyType
    {
        TS_PlayerEnter,
        TS_MonsterClear,
        TS_PlayerExit,
    }

    public enum TriggerActivateType
    {
        TA_EnterArena,
        TA_ExitArena,
        TA_SpawnComponent,
        TA_DirectingSubCam,
        TA_SetConfiner,
        TA_PlayInGameCutScene,
        TA_ActiveTrap,
        TA_DeactiveTrap,
        TA_SetGameObject,
        TA_LoadSector,
        TA_ExitSector,
        TA_SceneMusic,
        TA_StopSceneMusic,
        TA_AddStat,
        TA_SceneMusicFading,
        TA_EnterMapBox,
        TA_MonsterLab
    }

    [Serializable]
    public struct TriggerStrategyStruct
    {
        public TriggerStrategyType StrategyType;
    }

    [Serializable]
    public struct TriggerActivateStruct
    {
        public TriggerActivateType ActivateType;

        [ShowIf("ActivateType", TriggerActivateType.TA_EnterArena)]
        public List<string> BGMAddress;
        [ShowIf("ActivateType", TriggerActivateType.TA_EnterArena)]
        public string EnterSFXAddress;
        [ShowIf("ActivateType", TriggerActivateType.TA_ExitArena)]
        public string ExitSFXAddress;
        [ShowIf("ActivateType", TriggerActivateType.TA_EnterArena)]
        public float ArenaBGMFadeInTime;
        [ShowIf("ActivateType", TriggerActivateType.TA_ExitArena)]
        public float ArenaBGMFadeOutTime;
        
        
        [ShowIf("ActivateType", TriggerActivateType.TA_DirectingSubCam)] [Tooltip("0은 서브캠 해제")]
        public int subCamId;

        [ShowIf("ActivateType", TriggerActivateType.TA_SpawnComponent)]
        public GameObject componentGroup;

        [ShowIf("ActivateType", TriggerActivateType.TA_SetConfiner)]
        public bool isPoly;

        [ShowIf("ActivateType", TriggerActivateType.TA_SetConfiner)]  [Tooltip("null은 제한자 해제")]
        public PolygonCollider2D polygonCollider;

        [ShowIf("ActivateType", TriggerActivateType.TA_SetConfiner)] [Tooltip("null은 제한자 해제")]
        public BoxCollider2D boxCollider;
        
        [InfoBox("맵 전역 confiner를 제외하면 전부 1이상으로 맞춰주세요. 섹터 confiner등을 0으로 설정할 시 꼬일 수 있습니다.")]
        [ShowIf("ActivateType", TriggerActivateType.TA_SetConfiner)]
        public int priority; // 높은게 우선적.
        
        [ShowIf("ActivateType", TriggerActivateType.TA_SetConfiner)]
        public bool isOn; 
        [ShowIf("ActivateType", TriggerActivateType.TA_SetConfiner)]
        public bool resetConfiner; 
        
        [ShowIf("ActivateType", TriggerActivateType.TA_PlayInGameCutScene)]
        public int inGameCutSceneId;

        [ShowIf("ActivateType", TriggerActivateType.TA_PlayInGameCutScene)]
        public bool dontSave;

        [ShowIf("ActivateType", TriggerActivateType.TA_PlayInGameCutScene)]
        public UnityEvent endEvent;
        
        [ShowIf("ActivateType", TriggerActivateType.TA_SetGameObject)]
        public GameObject gameObject;
        [ShowIf("ActivateType", TriggerActivateType.TA_SetGameObject)]
        public bool active;
        

        [ShowIf("ActivateType", TriggerActivateType.TA_LoadSector)]
        [InfoBox("LoadSector를 설정했을 시, Trigger를 PlayerExit으로 하나 더 추가하여 ExitSector도 넣어주세요.")]
        [LabelText("섹터 id")]public int sectorID;
       
        [ShowIf("ActivateType", TriggerActivateType.TA_ExitSector)]
        [LabelText("섹터 id")]
        public int exitSectorID;
        [InfoBox("이 섹터의 주변 섹터들의 id")]
        [ShowIf("ActivateType", TriggerActivateType.TA_LoadSector)]
        public int[] injectionIDs;

        [ShowIf("ActivateType", TriggerActivateType.TA_SceneMusic)]
        public TA_SceneMusic.SceneBGMInfo bgmInfo;
        
        [ShowIf("ActivateType", TriggerActivateType.TA_AddStat)]
        public PermanentGrowthSaveData.PlayerData playerData;

        [ShowIf("ActivateType", TriggerActivateType.TA_SceneMusicFading)]
        public int number;
        [ShowIf("ActivateType", TriggerActivateType.TA_SceneMusicFading)]
        public float fadeTime;

        [ShowIf("ActivateType", TriggerActivateType.TA_EnterMapBox)]
        public int mapBoxId;
        
        [ShowIf("ActivateType", TriggerActivateType.TA_MonsterLab)]
        public List<int> monsterId;

        [ShowIf("ActivateType", TriggerActivateType.TA_MonsterLab)]
        public string effectAddress;
    }

    public enum TimeCheckType
    {
        ApplyTimeScale,
        IgnoreTimeScale
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Trigger : MonoBehaviour
    {
        
        public int triggerId;
        public bool isRepeat;
        public TimeCheckType timeType;
        public List<int> preTriggerIds;

        public TriggerStrategyStruct strategyData;
        public TriggerActivateStruct[] activatedDatas;
        public UnityEvent OnActivate;
        
        private ITriggerStrategy _triggerStrategy;
        private List<ITriggerActivate> _triggerActivates;

        public Collider2D _col;

        private Rigidbody2D rigid;
        public bool Triggered { get; private set; }

        public bool useFade;

        public bool useTaskCondition;
        [ShowIf("useTaskCondition")] public int taskIndex;
        
        private void Awake()
        {
            _triggerActivates = new();

            InitTrigger();
            rigid = gameObject.GetOrAddComponent<Rigidbody2D>();
            rigid.bodyType = RigidbodyType2D.Static;
            if (_col == null)
            {
                _col = GetComponent<Collider2D>();
            }
        }

        private void InitTrigger()
        {
            switch (strategyData.StrategyType)
            {
                case TriggerStrategyType.TS_MonsterClear:
                    _triggerStrategy = new TS_MonsterClear(this);
                    break;
                case TriggerStrategyType.TS_PlayerEnter:
                    _triggerStrategy = new TS_PlayerEnter(this);
                    break;
                case TriggerStrategyType.TS_PlayerExit:
                    _triggerStrategy = new TS_PlayerExit(this);
                    break;
                default:
                    Debug.Log("trigger strategy type 설정 안됨");
                    break;
            }

            _triggerActivates.Clear();
            foreach (var activate in activatedDatas)
            {
                switch (activate.ActivateType)
                {
                    case TriggerActivateType.TA_EnterArena:
                        _triggerActivates.Add(new TA_EnterArena(this,activate.BGMAddress,activate.ArenaBGMFadeInTime,activate.EnterSFXAddress));
                        break;
                    case TriggerActivateType.TA_ExitArena:
                        _triggerActivates.Add(new TA_ExitArena(this,activate.ArenaBGMFadeOutTime,activate.ExitSFXAddress));
                        break;
                    case TriggerActivateType.TA_SpawnComponent:
                        _triggerActivates.Add(new TA_SpawnComponent(this, activate.componentGroup));
                        break;
                    case TriggerActivateType.TA_DirectingSubCam:
                        _triggerActivates.Add(new TA_DirectingSubCam(this, activate.subCamId));
                        break;
                    case TriggerActivateType.TA_SetConfiner:
                        _triggerActivates.Add(activate.isPoly
                            ? new TA_ConfinerToggle(this, activate.polygonCollider, activate.priority, activate.isOn)
                            : new TA_ConfinerToggle(this, activate.boxCollider, activate.priority, activate.isOn));
                        break;
                    case TriggerActivateType.TA_PlayInGameCutScene:
                        _triggerActivates.Add(new TA_PlayInGameCutScene(activate.inGameCutSceneId,activate.dontSave,activate.endEvent));
                        break;
                    case TriggerActivateType.TA_ActiveTrap:
                        _triggerActivates.Add(new TA_ActiveTrap(this));
                        break;
                    case TriggerActivateType.TA_DeactiveTrap:
                        _triggerActivates.Add(new TA_DeactiveTrap(this));
                        break;
                    case TriggerActivateType.TA_SetGameObject:
                        _triggerActivates.Add(new TA_SetGameObject(activate.gameObject,activate.active));
                        break;
                    case TriggerActivateType.TA_LoadSector:
                        _triggerActivates.Add(new TA_EnterSector(activate.sectorID,activate.injectionIDs, this));
                        break;
                    case TriggerActivateType.TA_ExitSector:
                        _triggerActivates.Add(new TA_ExitSector(activate.exitSectorID));
                        break;
                    case TriggerActivateType.TA_SceneMusic:
                        _triggerActivates.Add(new TA_SceneMusic(activate.bgmInfo));
                        break;
                    case TriggerActivateType.TA_StopSceneMusic:
                        _triggerActivates.Add(new TA_StopSceneMusic());
                        break;
                    case TriggerActivateType.TA_AddStat:
                        _triggerActivates.Add(new TA_AddStat(activate.playerData));
                        break;
                    case TriggerActivateType.TA_SceneMusicFading:
                        _triggerActivates.Add(new TA_SceneMusicFading(activate.number,activate.fadeTime));
                        break;
                    case TriggerActivateType.TA_EnterMapBox:
                        _triggerActivates.Add(new TA_EnterMapBox(activate.mapBoxId));
                        break;
                    case TriggerActivateType.TA_MonsterLab:
                        _triggerActivates.Add(new TA_MonsterLab(this,activate.monsterId,activate.effectAddress));
                        break;
                    default:
                        Debug.LogError("trigger active type 설정 안됨");
                        break;
                }
            }
            
            if (!IsCanTriggered())
            {
                _col.enabled = false;
            }
        }

        bool CheckPreTriggers()
        {
            if (preTriggerIds.Count == 0) return true;

            return preTriggerIds.All(x => GameManager.Trigger.CheckActivated(x));
        }
        private bool IsCanTriggered()
        {
            return CheckPreTriggers() && (!useTaskCondition || !DataAccess.TaskData.IsDone(taskIndex));
        }
        
        public void ActivateTrigger()
        {
            if ((!isRepeat && Triggered) || !IsCanTriggered()) return;
            
            Triggered = true;

            if (useFade)
            {
                FadeManager.instance.Fading(() =>
                {
                    foreach (var trigger in _triggerActivates)
                    {
                        trigger.Activate();
                    }

                    OnActivate?.Invoke();
                    GameManager.Trigger.ActivateTrigger(triggerId);

                }, null, 0.2f);
            }
            else
            {
                foreach (var trigger in _triggerActivates)
                {
                    trigger.Activate();
                }

                OnActivate?.Invoke();
                GameManager.Trigger.ActivateTrigger(triggerId);
            }

        }

        private Collider2D[] result;
        private HashSet<Collider2D> colliders = new();
        private List<Collider2D> preColliders;

        // private Vector2 position, size;
        private int findCol;
        private void Update()
        {
            _col.enabled = IsCanTriggered();
        }
        
        public void OnTriggerEnter2D(Collider2D col)
        {
            if ((!isRepeat && Triggered) || !IsCanTriggered()) return;
            if (timeType == TimeCheckType.IgnoreTimeScale && colliders.Contains(col))
                return;
            colliders.Add(col);

            if (_triggerStrategy?.CheckAvailable(col) ?? false)
            {
                _triggerStrategy?.OnTriggerEnter2D(col);
                if (col.transform.parent.TryGetComponent(out IPhysicsTransition actorCollisionHandler))
                {
                    actorCollisionHandler.PhysicsTransitionHandler.ActivatingList.Add(this);
                }
            }
        }

        /**
         * 만약 Stay도 사용한다면 주석 처리 off
         * public void OnTriggerStay2D(Collider2D col) => _triggerStrategy.OnTriggerStay2D(col);
        **/
        public void OnTriggerExit2D(Collider2D col)
        {
            if ((!isRepeat && Triggered) || !IsCanTriggered()) return;
            if (timeType == TimeCheckType.IgnoreTimeScale && !colliders.Contains(col))
                return;
            colliders.Remove(col);
            
            IPhysicsTransition actorCollisionHandler = null;
            if (col.transform.parent != null)
            {
                actorCollisionHandler = col.transform.parent.GetComponent<IPhysicsTransition>();
            }
            
            if (actorCollisionHandler != null && actorCollisionHandler.PhysicsTransitionHandler.IgnoredList.Contains(this))
            {
                return;
            }

            if (_triggerStrategy?.CheckAvailable(col) ?? false)
            {
                if (actorCollisionHandler != null)
                {
                    actorCollisionHandler.PhysicsTransitionHandler.ActivatingList.Remove(this);
                }
                _triggerStrategy?.OnTriggerExit2D(col);
            }
        }
    }
}