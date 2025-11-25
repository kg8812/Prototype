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
        TS_PlayerExit,
    }

    public enum TriggerActivateType
    {
        TA_SpawnComponent,
        TA_SetGameObject,
        TA_SceneMusic,
        TA_StopSceneMusic,
        TA_SceneMusicFading,
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

        
        [ShowIf("ActivateType", TriggerActivateType.TA_SpawnComponent)]
        public GameObject componentGroup;
        
        [ShowIf("ActivateType", TriggerActivateType.TA_SetGameObject)]
        public GameObject gameObject;
        [ShowIf("ActivateType", TriggerActivateType.TA_SetGameObject)]
        public bool active;

        [ShowIf("ActivateType", TriggerActivateType.TA_SceneMusic)]
        public TA_SceneMusic.SceneBGMInfo bgmInfo;

        [ShowIf("ActivateType", TriggerActivateType.TA_SceneMusicFading)]
        public int number;
        [ShowIf("ActivateType", TriggerActivateType.TA_SceneMusicFading)]
        public float fadeTime;
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
                    case TriggerActivateType.TA_SpawnComponent:
                        _triggerActivates.Add(new TA_SpawnComponent(this, activate.componentGroup));
                        break;
                    case TriggerActivateType.TA_SetGameObject:
                        _triggerActivates.Add(new TA_SetGameObject(activate.gameObject,activate.active));
                        break;
                    case TriggerActivateType.TA_SceneMusic:
                        _triggerActivates.Add(new TA_SceneMusic(activate.bgmInfo));
                        break;
                    case TriggerActivateType.TA_StopSceneMusic:
                        _triggerActivates.Add(new TA_StopSceneMusic());
                        break;
                    case TriggerActivateType.TA_SceneMusicFading:
                        _triggerActivates.Add(new TA_SceneMusicFading(activate.number,activate.fadeTime));
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
            return CheckPreTriggers();
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