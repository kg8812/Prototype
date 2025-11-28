using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Default;
using DG.Tweening;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public struct AttackObjectInfo
    {
        public IAttackable attacker;
        public IAttackStrategy atkStrategy;
        public float duration;

        public AttackObjectInfo(
            IAttackable attacker,
            IAttackStrategy atkStrategy,
            float duration = 0
        )
        {
            this.attacker = attacker;
            this.atkStrategy = atkStrategy;
            this.duration = duration;
        }
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public partial class AttackObject : MonoBehaviour, IEventUser // 공격 오브젝트 (이펙트)
    {
        [HideInInspector] public GameObject firstAttackedTarget;

        [HideInInspector] public float duration;
        [HideInInspector] public string hitEffect;
        [HideInInspector] public bool isCrit;
        private List<Func<EventParameters, Tween>> _attackSequence;
        private Dictionary<AttackTypeEnum, IAttackType> atkDictionary;

        protected Sequence destroySeq;
        private bool isAttacked;

        private List<IOnHit> targets;

        private float tempFirstDmg;

        /// <summary>
        ///     tween을 반환하는 Action을 추가하면 각 Tween이 끝나면 순서대로 실행됩니다.
        /// </summary>
        public List<Func<EventParameters, Tween>> AttackSequence =>
            _attackSequence ??= new List<Func<EventParameters, Tween>>();

        protected virtual RigidbodyType2D BodyType => RigidbodyType2D.Kinematic;

        protected virtual void Awake()
        {
            SetAtkDictionary();

            Init(projectileInfo);
            targets = new List<IOnHit>();
            EventChildren.ForEach(x => x.Init(this));
            rigid.bodyType = BodyType;
            rigid.gravityScale = 0;
        }

        protected virtual void OnEnable()
        {
            AttackType.OnEnable();
            Collider.enabled = true;
        }

        protected virtual void OnDisable()
        {
        }

        public virtual void OnTriggerEnter2D(Collider2D other)
        {
        }

        public virtual void OnTriggerExit2D(Collider2D other)
        {
        }

        public void ChangeAttackType(AttackTypeEnum type)
        {
            AttackType?.OnExit();
            atkTypeEnum = type;
            _attackType = atkDictionary[type];
            AttackType?.OnEnter();
        }

        private void SetAtkDictionary()
        {
            atkDictionary ??= new Dictionary<AttackTypeEnum, IAttackType>
            {
                { AttackTypeEnum.Normal, new NormalAttack(this) },
                { AttackTypeEnum.Once, new OnceAttack(this) },
                { AttackTypeEnum.Delay, new DelayContinuousAttack(this, delayTime) },
                { AttackTypeEnum.Tick, new TickAttack(this, frequency) },
                { AttackTypeEnum.Cd, new CDAttack(this, cd) },
                { AttackTypeEnum.OnlyFirst, new OnlyFirst(this) }
            };
        }

        public bool CheckLayer(GameObject obj)
        {
            return (targetLayer.value & (1 << obj.layer)) > 0;
        }

        public bool CheckAlive(IOnHit target)
        {
            return !target.IsDead;
        }

        public bool CheckTarget(IOnHit target)
        {
            return CheckLayer(target.gameObject) && CheckAlive(target);
        }

        public virtual void Init(ProjectileInfo info)
        {
            if (info == null) return;

            isHitReaction = info.isHitReaction;
            hitReactionType = info.hitReactionType;
            knockBackData = info.knockBackData;
            KnockBackData =
                info.isSameDefaultKnockBack ? info.knockBackData : info.KnockBackData;
            attackType = info.attackType;
            targetLayer = info.targetLayer;
            AdditionalAtkCount = info.AdditionalAtkCount;
            ChangeAttackType(info.atkTypeEnum);
            Frequency = info.frequency;
            DelayTime = info.delayTime;
            CD = info.cd;
            isAtk = info.isAtk;
            firstDmg = info.firstDmg;
            first = info.first;
            duration = info.duration;
            AttackType.OnInit();
        }

        // 인수로 넉백을 넘기지 않은 경우, attackObject 인스펙터에 입력되어있는 값으로 진행
        /// <summary>
        ///     새로운 공격을 할 때마다 호출해야합니다.
        /// </summary>
        /// <param name="attacker">공격을 실행하는 Actor</param>
        /// <param name="atkStrategy">데미지 적용 전략 (공격력 비율, 상대체력 비율 등)</param>
        /// <param name="_duration">지속시간 (0일시 무한지속)</param>
        public void Init(IAttackable attacker, IAttackStrategy atkStrategy, float _duration = 0)
        {
            Init(new AttackObjectInfo(attacker, atkStrategy, _duration));
        }

        public virtual void Init(AttackObjectInfo atkObjectInfo)
        {
            ExecuteEvent(EventType.OnInit, new EventParameters(this));
            AttackSequence.Clear();

            atkObjInfo = atkObjectInfo;

            // 밑의 두 개는 atkObjInfo에 포함되어있지만 많이 쓰기에 꺼내둠.
            _attacker = atkObjectInfo.attacker;
            _atkStrategy = atkObjectInfo.atkStrategy;
            _eventUser = _attacker.gameObject.GetComponent<IEventUser>();
            _direction = _attacker.gameObject.GetComponent<IDirection>();
            _onHit = _attacker.gameObject.GetComponent<IOnHit>();

            if (!Mathf.Approximately(atkObjectInfo.duration, 0)) Destroy(atkObjectInfo.duration);

            Init(projectileInfo);

            AttackType.OnInit();
            hitEffect = null;
            firstAttackedTarget = null;
            isCrit = false;
            tempFirstDmg = 100;
            targets?.Clear();
            isAttacked = false;
            rigid.bodyType = BodyType;
            rigid.gravityScale = 0;
            firedAtkGuid = Guid.Empty;
        }

        public void DoAttackInvoke(EventParameters parameters, float dmgRatio = 100) // 공격(충돌시 호출) 가상 함수
        {
            tempFirstDmg = dmgRatio;
            AttackInvoke(parameters);
        }

        protected virtual void AttackInvoke(EventParameters parameters) // 공격(충돌시 호출) 가상 함수
        {
            if (parameters?.target == null) return;
            var isTargetFirstAttack = false;

            if (!targets.Contains(parameters.target))
            {
                targets.Add(parameters.target);
                ExecuteEvent(EventType.OnTargetFirstAttack, parameters);
                isTargetFirstAttack = true;
            }

            ExecuteEvent(EventType.OnAttackSuccess, parameters);

            var seq = DOTween.Sequence();
            var actions = AttackSequence.ToList();

            foreach (var unityAction in actions)
            {
                var tween = unityAction.Invoke(parameters);
                tween?.Pause();
                if (tween != null) seq.Append(tween);
            }

            if (isAtk)
            {
                Attack(parameters, DmgRatio * tempFirstDmg / 100);
                if (isTargetFirstAttack)
                    _eventUser?.EventManager.ExecuteEvent(EventType.OnTargetFirstAttack, parameters);
            }
        }

        private void DoAdditionalAtk(EventParameters parameters)
        {
            GameManager.instance.StartCoroutine(AdditionalAtkCoroutine(AdditionalAtkCount, parameters));
        }

        private IEnumerator AdditionalAtkCoroutine(int count, EventParameters parameters)
        {
            for (var i = 0; i < count; i++)
            {
                float curTime = 0;
                while (curTime < 0.05f)
                {
                    curTime += Time.deltaTime;
                    yield return new WaitForEndOfFrame();
                }

                var tempParam = new EventParameters(parameters.user, parameters.target)
                {
                    collideData = new CollideEventData { collider = parameters.collideData.collider }
                };

                DoAttackInvoke(tempParam);
            }
        }

        public EventParameters Attack(EventParameters targetParameters)
        {
            targetParameters.Reset();
            targetParameters.atkData.isHitReaction = isHitReaction;
            targetParameters.atkData.atkStrategy = _atkStrategy;
            targetParameters.atkData.attackType = attackType;
            targetParameters.atkData.attackGuid = firedAtkGuid;
            // targetParameters.atkData.hitPoint = Position;
            if (isCrit) targetParameters.atkData.isfixedCrit = true;

            // targetParameters.knockBackData.knockBackForce = atkObjInfo.knockBackForce;
            // targetParameters.knockBackData.knockBackTime = atkObjInfo.knockBackTime;
            // targetParameters.knockBackData.knockBackAngle = atkObjInfo.knockBackAngle;

            targetParameters.knockBackData = knockBackData;
            targetParameters.KnockBackData = KnockBackData;
            targetParameters.master = _eventUser;

            firstAttackedTarget ??= targetParameters.target.gameObject;

            var parameters = _attacker.Attack(targetParameters);
            if (!isAttacked)
            {
                if (targetParameters.atkData.attackType == Define.AttackType.BasicAttack)
                    _eventUser?.EventManager.ExecuteEvent(EventType.OnFirstAttack, parameters);

                isAttacked = true;
            }

            if (parameters == null) return null;

            if (!Mathf.Approximately(parameters.hitData.dmgReceived, 0))
            {
                ExecuteEvent(EventType.OnAfterAtk, parameters);
                if (hitEffect != null)
                {
                    var effect = GameManager.Factory.Get<ParticleSystem>(FactoryManager.FactoryType.Effect,
                        hitEffect, parameters.target.Position);
                    GameManager.Factory.Return(effect.gameObject, effect.main.duration);
                }
            }

            return parameters;
        }

        protected EventParameters Attack(EventParameters targetParameters, float dmgRatio)
        {
            var temp = DmgRatio;
            DmgRatio = dmgRatio;
            var parameters = Attack(targetParameters);
            DmgRatio = temp;
            return parameters;
        }

        /// <summary>
        ///     파괴 함수, 공격 오브젝트는 무조건 이 함수를 호출해서 파괴할 것
        /// </summary>
        public virtual void Destroy()
        {
            if (gameObject == null || !gameObject.activeSelf) return;

            destroySeq?.Kill();
            destroySeq = null;
            AttackType.OnDisable();
            ExecuteEvent(EventType.OnDestroy, new EventParameters(this));
            AttackSequence.Clear();
            isCrit = false;
            isAttacked = false;
            targets?.Clear();
            Collider.enabled = false;
            if (gameObject.TryGetComponent(out ParticleDestroyer destroyer))
                destroyer.StopEmitting();
            else
                //바로 돌려보내니까 가끔 위치 버그가 나서 비활성화만 시키고 반환은 2프레임 이후로 설정함.
                Utils.ActionAfterTime(() => GameManager.Factory.Return(gameObject), Time.deltaTime * 2);
        }

        /// <summary>
        ///     파괴 함수, 공격 오브젝트는 무조건 이 함수를 호출해서 파괴할 것
        /// </summary>
        /// <param name="time">딜레이 시간</param>
        /// <param name="action">파괴 후 실행할 함수</param>
        /// <returns></returns>
        public Sequence Destroy(float time, List<TweenCallback> action = null)
        {
            destroySeq?.Kill();
            destroySeq = DOTween.Sequence();
            destroySeq.SetDelay(time);
            action?.ForEach(x => destroySeq.AppendCallback(x));
            destroySeq.AppendCallback(Destroy);

            return destroySeq;
        }

        /// <summary>
        ///     이벤트 추가 (Init 혹은 Destroy될 때 제거됨)
        /// </summary>
        public void AddEventUntilInitOrDestroy(UnityAction<EventParameters> action,
            EventType type = EventType.OnAttackSuccess)
        {
            switch (type)
            {
                case EventType.OnDestroy:
                    AddEvent(EventType.OnDestroy, ActionDestroy);
                    break;
                case EventType.OnInit:
                    AddEvent(EventType.OnInit, ActionInit);
                    break;
                default:
                    AddEvent(type, action);
                    AddEvent(EventType.OnDestroy, Remove);
                    AddEvent(EventType.OnInit, Remove);
                    break;
            }

            void Remove(EventParameters info)
            {
                RemoveEvent(type, action);
                RemoveEvent(EventType.OnDestroy, Remove);
                RemoveEvent(EventType.OnInit, Remove);
            }

            void ActionDestroy(EventParameters info)
            {
                action.Invoke(info);
                RemoveEvent(EventType.OnDestroy, ActionDestroy);
            }

            void ActionInit(EventParameters info)
            {
                action.Invoke(info);
                RemoveEvent(EventType.OnInit, ActionInit);
            }
        }

        /// <summary>
        ///     공격 한 번 하면 사라지는 이벤트
        /// </summary>
        public void AddAtkEventOnce(UnityAction<EventParameters> action)
        {
            void TempAction(EventParameters x)
            {
                action(x);
            }

            void Remove(EventParameters info)
            {
                RemoveEvent(EventType.OnAttackSuccess, TempAction);
                RemoveEvent(EventType.OnDestroy, Remove);
                RemoveEvent(EventType.OnInit, Remove);
            }

            AddEvent(EventType.OnAttackSuccess, TempAction);
            AddEvent(EventType.OnAttackSuccess, Remove);
            AddEvent(EventType.OnDestroy, Remove);
            AddEvent(EventType.OnInit, Remove);
        }

        #region Events

        public IEventManager EventManager => BuffEvent;

        private List<IEventChild> _eventChildren;

        public List<IEventChild> EventChildren => _eventChildren ??= new List<IEventChild>
        {
            BuffEvent, CollisionEventHandler
        };

        private CollisionEventHandler _collisionEventHandler;

        private CollisionEventHandler CollisionEventHandler =>
            _collisionEventHandler ??= gameObject.GetOrAddComponent<CollisionEventHandler>();

        private BuffEvent _buffEvent;
        public BuffEvent BuffEvent => _buffEvent ??= gameObject.GetOrAddComponent<BuffEvent>();

        public void AddEvent(EventType eventType, UnityAction<EventParameters> action)
        {
            BuffEvent.AddEvent(eventType, action);
        }

        public void RemoveEvent(EventType eventType, UnityAction<EventParameters> action)
        {
            BuffEvent.RemoveEvent(eventType, action);
        }

        public void ExecuteEvent(EventType eventType, EventParameters parameters)
        {
            BuffEvent.ExecuteEvent(eventType, parameters);
        }

        #endregion

        #region Inspector

        private bool CheckInfo()
        {
            return projectileInfo != null;
        }

        [ShowIfGroup("InfoGroup", Condition = "@projectileInfo == null")]
        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")]
        [LabelText("피격 반응 여부")]
        public bool isHitReaction;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("피격 반응 유형")]
        public AttackEventData.HitReactionType hitReactionType;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("넉백 파워")] [Tooltip("만약 0이라면 없는것으로 간주됩니다.")]
        public float knockBackForce;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("넉백 시간")] [Tooltip("넉백으로 이동하는 시간입니다.")]
        public float knockBackTime;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("넉백 각도")]
        public float knockBackAngle;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("일반 넉백")]
        public KnockBackData knockBackData;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("그로기 넉백")]
        public KnockBackData KnockBackData;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("타겟 레이어")]
        public LayerMask targetLayer;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("공격 여부")]
        public bool isAtk = true;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("추가공격 횟수 (다단히트시)")]
        public int AdditionalAtkCount;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("공격타입")] [SerializeField]
        private AttackTypeEnum atkTypeEnum;

        [ShowIf("atkTypeEnum", AttackTypeEnum.Tick)]
        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")]
        [LabelText("공격주기")]
        [SerializeField]
        private float frequency;

        [ShowIf("atkTypeEnum", AttackTypeEnum.Tick)]
        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")]
        [LabelText("첫 공격 데미지(%)")]
        [InfoBox("기존 데미지의 %로 들어감" +
                 "\n 기존이 50%, 이 값이 50%일시 25% 데미지")]
        [SerializeField]
        private float firstDmg;

        [ShowIf("atkTypeEnum", AttackTypeEnum.Tick)]
        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")]
        [LabelText("첫 공격 그로기(%)")]
        [SerializeField]
        private float first = 100;

        [ShowIf("atkTypeEnum", AttackTypeEnum.Delay)]
        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")]
        [LabelText("딜레이 시간")]
        [SerializeField]
        private float delayTime;

        [ShowIf("atkTypeEnum", AttackTypeEnum.Cd)]
        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")]
        [LabelText("쿨타임")]
        [SerializeField]
        private float cd;

        [TabGroup("InfoGroup/공격설정", "공격 오브젝트 설정")] [LabelText("공격판정")] [SerializeField]
        private Define.AttackType attackType;

        [LabelText("오브젝트 설정")] public ProjectileInfo projectileInfo;

        #endregion

        #region Properties

        public IAttackable _attacker;
        public IEventUser _eventUser;
        public IDirection _direction;
        public IOnHit _onHit;

        private Collider2D _collider;

        public Collider2D Collider => _collider ??= transform.GetComponentInParentAndChild<Collider2D>();

        private Rigidbody2D _rigid;
        public Rigidbody2D rigid => _rigid ??= GetComponent<Rigidbody2D>();

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Guid firedAtkGuid; // 어느 공격에서 생긴건지 파악하기 위한 변수 (중복 데미지 감소 등 적용 위함)
        public Vector3 TopPivot { get; set; }

        public float DmgRatio
        {
            get => _atkStrategy?.DmgRatio ?? 100;
            set
            {
                if (_atkStrategy != null) _atkStrategy.DmgRatio = value;
            }
        }

        public AttackObjectInfo atkObjInfo;

        protected IAttackStrategy _atkStrategy;

        private IAttackType _attackType;

        public IAttackType AttackType
        {
            get
            {
                if (_attackType != null) return _attackType;

                SetAtkDictionary();
                _attackType = atkDictionary[atkTypeEnum];
                _attackType?.OnEnter();

                return _attackType;
            }
        }


        public float Frequency
        {
            get => frequency;
            set
            {
                frequency = value;
                if (AttackType is TickAttack tick)
                {
                    tick.frequency = value;
                    tick.OnInit();
                }
            }
        }

        public float DelayTime
        {
            get => delayTime;
            set
            {
                delayTime = value;
                if (AttackType is DelayContinuousAttack a) a.delayTime = value;
            }
        }

        public float CD
        {
            get => cd;
            set
            {
                cd = value;
                if (AttackType is CDAttack a) a.cd = value;
            }
        }

        #endregion
    }
}