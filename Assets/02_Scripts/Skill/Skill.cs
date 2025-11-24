using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.Managers;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Apis
{
    public abstract partial class Skill : SerializedScriptableObject , IAttackItemStat
    {
        
        #region Inspector
        [TitleGroup("사용 관련")] 
        
        [TabGroup("사용 관련/설정","기본설정")]
        [LabelText("쿨타임 적용 시기")] [SerializeField] protected CdStartType _cdStartType = CdStartType.OnUse;
        [TabGroup("사용 관련/설정","기본설정")][LabelText("쿨타임")] [SerializeField] float baseCd;
        [TabGroup("사용 관련/설정","기본설정")][LabelText("지속시간")] [SerializeField] private float duration;
        [TabGroup("사용 관련/설정","기본설정")][LabelText("모션여부")] [SerializeField] protected bool isMotion;
        [ShowIf("isMotion")] [TabGroup("사용 관련/설정", "기본설정")][LabelText("스킬모션 인덱스")] [SerializeField]
        protected int index;
        [ShowIf("isMotion")] [TabGroup("사용 관련/설정", "기본설정")] [LabelText("스킬타입")][SerializeField]
        private SkillMotionType skillMotionType;
        [FormerlySerializedAs("MaxStack")]
        [ShowIf("_cdType",CDEnums.Stack)]
        [LabelText("최대 스택")][TabGroup("사용 관련/설정","기본설정")]
        [SerializeField] int maxStack;

        [ShowIf("_cdType", CDEnums.Stack)] [LabelText("사용 최소 쿨타임")] [TabGroup("사용 관련/설정", "기본설정")]
        public float minStackCd;
        
        [LabelText("시작 쿨타임 적용여부")] [SerializeField][TabGroup("사용 관련/설정","기본설정")]
        private bool cdStart;

        [LabelText("장착해제시 쿨타임 활성화 여부")] [SerializeField] [Tooltip("체크해제 : 장착해제시 쿨타임 동결")]
        [TabGroup("사용 관련/설정","기본설정")]private bool unEquipCd;
        [FormerlySerializedAs("EnterChargeState")]
        [TabGroup("사용 관련/설정", "기본설정")]
        [LabelText("스킬상태 들어가는 여부")]public bool EnterSkillState = true;

        [FormerlySerializedAs("dmg")]
        [TitleGroup("스탯값")] 
        [ShowIf("UseAtkRatio")]
        [TitleGroup("스탯값")][LabelText("스킬 공격력")] [Tooltip("스킬의 기본 수치")]
        [SerializeField] protected float baseDmg;
        [TitleGroup("스탯값")][LabelText("신체 피해%")] [SerializeField] private float bodyFactor;
        [TitleGroup("스탯값")][LabelText("영혼 피해%")] [SerializeField] private float spiritFactor;
        [TitleGroup("스탯값")][LabelText("기량 피해%")] [SerializeField] private float finesseFactor;
        public float BodyFactor => bodyFactor;

        public float SpiritFactor => spiritFactor;

        public float FinesseFactor => finesseFactor;
        [FormerlySerializedAs("groggy")]
        [ShowIf("UseGroggyRatio")] [TitleGroup("스탯값")] [PropertyOrder(1)] [LabelText("그로기 계수")]
        [SerializeField] protected float baseGroggy;

        [Title("string 인덱스")]
        [LabelText("이름")] [SerializeField] protected int nameString;
        [LabelText("설명")] [SerializeField] protected int descString;

        [LabelText("item용 id. 0이면 item없음.")] [SerializeField]
        public int itemId;

        #endregion
        
        #region 스탯값

        public float Cd
        {
            get
            {
                float value = stats?.Stat != null
                    ? (baseCd - stats.Stat.baseCd) / (1 + stats.Stat.baseCdRatio / 100)
                    : baseCd;
                
                if (statUser != null)
                {
                    value = FormulaConfig.CalculateCD(statUser.StatManager.GetFinalStat(ActorStatType.CDReduction),
                        value);
                }

                return Mathf.Clamp(value, 0.1f, value);
            }
        }

        public float Duration
        {
            get
            {
                if (stats?.Stat != null)
                {
                    return (duration + stats.Stat.duration) * (1 + stats.Stat.durationRatio / 100);
                }

                return duration;
            }
        }

        public virtual int MaxStack
        {
            get
            {
                if (stats?.Stat != null)
                {
                    return Mathf.RoundToInt((maxStack + stats.Stat.maxStack) * (1 + stats.Stat.maxStackRatio / 100));
                }

                return maxStack;
            }
        }

        public int StackGain
        {
            get
            {
                if (stats?.Stat != null)
                {
                    return stackGain + stats.Stat.stackGain;
                }

                return stackGain;
            }
        }

        public virtual float Atk
        {
            get
            {
                
                if (stats?.Stat != null)
                {
                    return (baseDmg + stats.Stat.dmg) * (1 + stats.Stat.dmgRatio / 100);
                }
                return baseDmg;
            }
        }

        public virtual float BaseGroggyPower
        {
            get
            {
                if (stats?.Stat != null)
                {
                    return (baseGroggy + stats.Stat.groggy) * (1 + stats.Stat.groggyRatio / 100);
                }

                return baseGroggy;
            }
        }

        #endregion
        
        #region 프로퍼티
        
        [HideInInspector] public IMonoBehaviour user;
        [HideInInspector] public IAnimator animator;
        [HideInInspector] public IAttackable attacker;
        [HideInInspector] public IMovable mover;
        [HideInInspector] public IOnHit hit;
        [HideInInspector] public IMecanimUser skeleton;
        [HideInInspector] public IEventUser eventUser;
        [HideInInspector] public IDirection direction;
        [HideInInspector] public IStatUser statUser;
        [HideInInspector] public IDashUser dashUser;
        [HideInInspector] public IActiveSkillUser activeUser;
        [HideInInspector] public IPassiveSkillUser passiveUser;
        [HideInInspector] public IWeaponSkillUser weaponSkillUser;

        protected virtual bool UseAtkRatio => true;
        protected virtual bool UseGroggyRatio => true;
        
        public virtual float cdRatio => 100;
        protected virtual bool CdUse => true;

        private UnityEvent<int> _onStackChange = new();
        public UnityEvent<int> OnStackChange => _onStackChange ??= new();

        private const int stackGain = 1;
        protected virtual CDEnums _cdType => CDEnums.Normal;
        public int SkillName => baseConfig.SkillName;
        public int Desc => baseConfig.Desc;
        
        private List<ISkill> _attachments;

        protected List<ISkill> attachments => _attachments ??= new();
        
        protected ISkill stats;
        protected SkillConfig baseConfig;
        
        public virtual int CurStack
        {
            get => _curStack;
            set
            {
                _curStack = Mathf.Clamp(value,0,MaxStack);
                OnStackChange.Invoke(_curStack);
            }
        }
        private int _curStack;

        private UnityEvent _onAfterDuration;
        
        public UnityEvent OnAfterDuration => _onAfterDuration ??= new();
        
        private ICdActive _cdActive;

        public virtual ICdActive CDActive
        {
            get
            {
                _cdActive ??= new NormalCd(this);
                return _cdActive;
            }
        }
        public virtual float CurCd
        {
            get => CDActive?.CurCd ?? 0;
            set
            {
                if (CDActive != null)
                {
                    CDActive.CurCd = value;
                }
            }
        }
        protected float curDuration;
        public float CurDuration => curDuration;
        protected bool IsDuration => durationCoroutine != null;

        [HideInInspector] public float CurCastTime;
        [HideInInspector] public float CurChargeTime;

        private bool isUse;

        public bool IsUse
        {
            get => isUse;
            protected set
            {
                isUse = value;
            }
        }

        private UnityEvent _onActive;
        public UnityEvent OnActive => _onActive ??= new();
        
         
        private UnityEvent _onUpdate = new();
        public UnityEvent OnUpdate => _onUpdate ??= new();

        public bool IsToggleOn => this is ActiveSkill { ActiveStrategy: ToggleSkill { isToggleOn: true } };
        #endregion

        #region Enums
        
        public enum SkillMotionType
        {
            Magic,Active,
        }
        protected enum CdStartType
        {
            OnUse,
            AfterCharge,
            AfterDuration,
            OnCancel
        };

        protected enum CDEnums
        {
            Normal,Stack
        }
        #endregion
        
        #region Events

        private Action _onSkillUse;
        public event Action OnSkillUse
        {
            add
            {
                _onSkillUse -= value;
                _onSkillUse += value;
            }
            remove => _onSkillUse -= value;
        }

        private Action _onSkillEquip;
        
        public event Action OnSkillEquip
        {
            add
            {
                _onSkillEquip -= value;
                _onSkillEquip += value;
            }
            remove => _onSkillEquip -= value;
        }

        private Action _onSkillUnEquip;
        
        public event Action OnSkillUnEquip
        {
            add
            {
                _onSkillUnEquip -= value;
                _onSkillUnEquip += value;
            }
            remove => _onSkillUnEquip -= value;
        }
        #endregion
        
        private bool isStop = false;
        
        public void Stop()
        {
            isStop = true;
        }

        public float CalculateDmg()
        {
            if (user is Actor _user)
            {
                return Atk + _user.Finesse * FinesseFactor / 100f +
                        _user.Body * BodyFactor / 100f + _user.Spirit * SpiritFactor / 100f;
            }

            return Atk;
        }
        protected virtual void UpdateEvent(EventParameters parameters)
        {
            if (isStop) return;

            if (CdUse)
            {
                CDActive.Update(parameters);
            }
            
            OnUpdate.Invoke();
        }

        public void AddAttachment(ISkill attachment)
        {
            if (attachments.Contains(attachment))
            {
                Debug.Log("이미 등록된 데코레이터");
                return;
            }
            attachments.Add(attachment);
            Decorate();
        }

        public void RemoveAttachment(ISkill attachment)
        {
            attachments.Remove(attachment);
            Decorate();
        }

        public virtual void Decorate()
        {
            stats = baseConfig;
            attachments.ForEach(x =>
            {
                stats = new SkillDecorator(stats, x);
            });
        }
        
        public abstract bool TryUse();


        protected virtual void SetConfig()
        {
            baseConfig = new SkillConfig(new SkillStat());
        }
        public virtual void Init()
        {
            IsUse = false;
            
            attachments.Clear();
            SetConfig();
            Decorate();
            
            baseConfig.SkillName = nameString;
            baseConfig.Desc = descString;
            CDActive.Init();

            if (cdStart)
            {
                CurStack = 0;
                CDActive.SetCD();
                CDActive.StartCd();
            }
            else
            {
                CurStack = maxStack;
                CurCd = 0;
            }
        }
        
        public virtual bool Use()
        {
            if (!TryUse()) return false;
            if (hit is { IsDead: true }) return false;
            
            CDActive.SetCD();
            if (_cdStartType == CdStartType.OnUse)
            {
                CDActive.StartCd();
            }
            return true;
        }

        public virtual void Active()
        {
            if (_cdStartType == CdStartType.AfterCharge)
            {
                CDActive.StartCd();
            }

            if (!Mathf.Approximately(Duration,0) && (this is not ActiveSkill a || a.ActiveStrategy.durationUse ))
            {
                StartDuration();
            }
            OnActive.Invoke();
            isStop = false;
            
            if (isMotion)
            {
                if (EnterSkillState && user is Player player)
                {
                    player.SetState(EPlayerState.Skill);
                }
                animator?.animator.SetBool("SkillAir", mover != null && !mover.ActorMovement.IsStick);
                animator?.animator.SetInteger("PlayerType", (int)GameManager.instance.Player.playerType);

                switch (skillMotionType)
                {
                    case SkillMotionType.Magic:
                        animator?.animator.ResetTrigger("WeaponSkillEnd");
                        animator?.animator.SetTrigger("WeaponSkill");
                        break;
                    
                    case SkillMotionType.Active:
                        TurnOnActiveMotion();
                        break;
                }
                AddCancelEvents();
            }
        }
        
        protected virtual void TurnOnActiveMotion()
        {
            animator?.animator.SetInteger("ActiveSkillType",index);
            animator?.animator.ResetTrigger("PlayerSkillEnd");
            animator?.animator.ResetTrigger("PlayerSkill");
            animator?.animator.SetTrigger("PlayerSkillInit");
        }
        protected virtual void StartDuration()
        {
            durationCoroutine = GameManager.instance.StartCoroutineWrapper(DurationCoroutine());
            RemoveCancelEvents();
        }

        public virtual IEnumerator DurationCoroutine()
        {
            IsUse = true;
            curDuration = Duration;

            while (curDuration > 0)
            {
                if (!isStop)
                {
                    curDuration -= Time.deltaTime;
                }

                yield return new WaitForEndOfFrame();
            }
            AfterDuration();
            
            IsUse = false;
            
            durationCoroutine = null;
        }

        public virtual void AfterDuration()
        {
            if (_cdStartType == CdStartType.AfterDuration)
            {
                CDActive.SetCD();
                CDActive.StartCd();
            }
            OnAfterDuration.Invoke();
            isStop = false;
        }

        protected virtual void StopDuration()
        {
            if (durationCoroutine != null)
            {
                GameManager.instance.StopCoroutineWrapper(durationCoroutine);
                curDuration = 0;
                AfterDuration();
                if (_cdStartType == CdStartType.AfterDuration)
                {
                    CDActive.StartCd();
                }

                durationCoroutine = null;
            }

            IsUse = false;
        }

        protected bool isEquip;
        
        public void Equip(IMonoBehaviour owner)
        {
            if (isEquip) return;
            isEquip = true;
            OnEquip(owner);
        }
        
        protected virtual void OnEquip(IMonoBehaviour owner)
        {
            user = owner;
            animator = owner.gameObject.GetComponent<IAnimator>();
            attacker = owner.gameObject.GetComponent<IAttackable>();
            mover = owner.gameObject.GetComponent<IMovable>();
            skeleton = owner.gameObject.GetComponent<IMecanimUser>();
            eventUser = owner.gameObject.GetComponent<IEventUser>();
            direction = owner.gameObject.GetComponent<IDirection>();
            statUser = owner.gameObject.GetComponent<IStatUser>();
            dashUser = owner.gameObject.GetComponent<IDashUser>();
            hit = owner.gameObject.GetComponent<IOnHit>();
            activeUser = owner.gameObject.GetComponent<IActiveSkillUser>();
            passiveUser = owner.gameObject.GetComponent<IPassiveSkillUser>();
            weaponSkillUser = owner.gameObject.GetComponent<IWeaponSkillUser>();

            eventUser?.EventManager.AddEvent(EventType.OnUpdate,UpdateEvent);
            isStop = false;
            _onSkillEquip?.Invoke();
            effectSpawner = new EffectSpawner(user as Actor);
        }

        public void UnEquip()
        {
            if (!isEquip) return;
            isEquip = false;
            OnUnEquip();
        }

        protected virtual void OnUnEquip()
        {
            if (!unEquipCd)
            {
                eventUser?.EventManager.RemoveEvent(EventType.OnUpdate, UpdateEvent);
            }

            Cancel();
            
            _onSkillUnEquip?.Invoke();
            RemoveCancelEvents();
            RemoveAllEffects();
        }

        private Coroutine durationCoroutine;
        public virtual void Cancel()
        {
            StopDuration();
            if (_cdStartType == CdStartType.OnCancel)
            {
                CDActive.StartCd();
            }
            RemoveCancelEvents();
        }
        
        public void TurnOnMotion()
        {
            isMotion = true;
        }

        public void TurnOffMotion()
        {
            isMotion = false;
            RemoveCancelEvents();
        }

        void AddCancelEvents()
        {
            if (IsDuration) return;
            eventUser?.EventManager.AddEvent(EventType.OnDash,CancelInvoke);
            eventUser?.EventManager.AddEvent(EventType.OnHitReaction,CancelInvoke);
        }

        void RemoveCancelEvents()
        {
            eventUser?.EventManager.RemoveEvent(EventType.OnDash,CancelInvoke);
            eventUser?.EventManager.RemoveEvent(EventType.OnHitReaction,CancelInvoke);
        }
        
        void CancelInvoke(EventParameters parameters)
        {
            Cancel();
        }
    }
}