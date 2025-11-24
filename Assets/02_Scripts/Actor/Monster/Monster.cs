using System;
using System.Collections;
using Apis;
using Apis.DataType;
using Apis.Interface;
using Apis.Managers;
using Default;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Apis 
{
    public enum MonsterType
    {
        Common,
        Elite,
        Boss
    }

    public delegate void OnFloatValueChanged(float value);

    public partial class Monster : Actor, IRecognition, IMovable, IPoolObject
    {
        // inspector 용
        public bool isAlreadyCreated = false;
        public int monsterId;
        
        [TabGroup("기획쪽 수정 변수들/group1", "몬스터 설정")] [LabelText("넉백 반응 여부")] [SerializeField]
        public bool canKnockBacked = true;
        [TabGroup("기획쪽 수정 변수들/group1", "몬스터 설정")] [LabelText("몬스터 데이터 테이블 반영 여부")] [SerializeField]
        private bool isStatLoadFromDataTable;
        [HideIf("isStatLoadFromDataTable", true)]
        public MonsterDataType MonsterData;

        private static ProjectileInfo _atkInfo;

        public static ProjectileInfo AtkInfo
        {
            get
            {
                return _atkInfo ??= ResourceUtil.Load<ProjectileInfo>("MonsterAtkInfo");
            }
        }

        // 참조 dragon
        public ItemDropper ItemDropper { get; private set; }
        public UnitMoveComponent MoveComponent
        {
            get
            {
                if (_moveComponent == null)
                {
                    _moveComponent = gameObject.GetOrAddComponent<MonsterMoveComponent>();
                    _moveComponent.Init(this,Collider);
                }

                return _moveComponent;
            }
        }
        public ActorMovement ActorMovement => MoveComponent?.ActorMovement;
        
        public bool ableMove
        {
            get => MoveComponent.ableMove;
            set => MoveComponent.ableMove = value;
        }
        public bool ableJump
        {
            get => MoveComponent.ableJump;
            set => MoveComponent.ableJump = value;
        }

        public virtual bool IsRecognized
        {
            get => _isRecognized;
            set
            {
                if (_isRecognized == value) return;
                _isRecognized = value;
                if (value)
                    OnRecognized();
                else
                    OnDisRecognized();
            }
        }
        
        
        // privates
        private bool _isRecognized;
        private MonsterMoveComponent _moveComponent;

        public override int Exp => MonsterData?.dropExp ?? base.Exp;

        #region Init

        protected override void Awake()
        {
            base.Awake();
            _isIntractable = false;
            OnAdd ??= new();
            OnRemove ??= new();
            OnAdd.AddListener(x => { x.Invoke(this); });
            OnRemove.AddListener(x => { x.Invoke(this); });
            ItemDropper = GetComponent<ItemDropper>();
            InteractCheckEvent -= Check;
            InteractCheckEvent += Check;
            
            AddEvent(EventType.OnHpDown, LastHit);
            ClearShaderOnDeath();

            // 현재는 모든 피해(CurHP가 줄어드는 Event)에 대하여 Groggy Gauge Recover 작동하도록 해둠.
            // off라면 나중에 event 말고 onhit에서 호춣하는 방식으로 수정
        }
     
        // 몬스터 팩토리에서 꺼내올때마다 호출
        public virtual void Init(MonsterDataType monsterDataType)
        {
            if (isStatLoadFromDataTable)
            {
                InitMonsterData(monsterDataType);
            }
            CurHp = MaxHp;
            Rb.gravityScale = MonsterData.isFlying ? 0 : MoveComponent.ActorMovement.GravityScale;
            IsDead = false;
            ClearShaderOnDeath();
            ResetTextVariables();
            
            // groggy 리셋
            if (recoverGroggyCoroutine != null)
            {
                StopCoroutine(recoverGroggyCoroutine);
                recoverGroggyCoroutine = null;
            }

            _curGroggyGauge = 0;
            IsGroggy = false;
            
            // onoff 리셋
            MoveCCOff();
            AttackOn();
            TurnFrozenOff();
            
            
            // animator 리셋
            animator.Rebind();
            animator.enabled = false;
            animator.enabled = true;

            IdleOn();
        }

        protected virtual void InitMonsterData(MonsterDataType monsterDataType)
        {
            this.MonsterData = monsterDataType;
            ItemDropper.DropperId = monsterDataType.dropIndex;
            StatManager.BaseStat.Set(ActorStatType.MaxHp,MonsterData.maxHp);
            StatManager.BaseStat.Set(ActorStatType.Atk,MonsterData.atkPower);
            StatManager.BaseStat.Set(ActorStatType.MoveSpeed, MonsterData.moveSpeed);
        }
        
        
        public override void IdleOn()
        {
        }

        #endregion

        #region Factory

        public virtual void OnGet()
        {
            Init(MonsterModel.monsterDict[monsterId]);
        }

        public virtual void OnReturn()
        {
        }

        #endregion


        // TODO: 저장이 바뀜에 따라 Progress Manager가 아직도 유효한지.
        public void Progressed()
        {
            // TODO: 게임 오브젝트 비활성화
            gameObject.SetActive(false);
        }


        #region Recognition

        public virtual void OnRecognized()
        {
            /*
             * 몬스터 인식 시.
             * Boss, Elite => Trigger에서 호출
             * Common => 자체적으로 Player Ray 쏴서 판단하고 호출
             */
            _isRecognized = true;
            GameManager.instance.BattleStateClass.AddRecogMonster(this);
            // 보스나 엘리트면 체력바 생성
            if (MonsterData?.monsterType != MonsterType.Common)
            {
                GameManager.instance.BattleStateClass.RegisterMonsterForHpBar(this);

                void Remove(EventParameters _)
                {
                    GameManager.instance.BattleStateClass.RemoveMonsterForHpBar(this);
                    RemoveEvent(EventType.OnDeath, Remove);
                }
                AddEvent(EventType.OnDeath, Remove);
            }
            ExecuteEvent(EventType.OnRecognitionEnter, new EventParameters(this));
        }

        public virtual void OnDisRecognized()
        {
            ExecuteEvent(EventType.OnRecognitionExit, new EventParameters(this));
        }

        #endregion

        #region Hit

        public override float OnHit(EventParameters parameters)
        {
            if (parameters.atkData.groggyAmount > 0)
            {
                AddGroggyGauge(parameters.atkData.groggyAmount);
            }
            return base.OnHit(parameters);
        }

        protected override void OnHitReaction(EventParameters eventParameters)
        {
            base.OnHitReaction(eventParameters);
            
            ShaderOnHit();
           
            // TODO: 여기에 피격 이펙트 넣을지, hit 자체에 넣을지
            KnockBackData knockBackData = GetKnockBackData(eventParameters);
            
            Vector2 knockBackSrc = knockBackData.directionType == KnockBackData.DirectionType.AktObjRelative
                ? eventParameters.user.Position
                : eventParameters.master.Position;
            if (eventParameters.atkData.isHitReaction && !HitImmune && knockBackData.knockBackForce != 0 &&
                canKnockBacked)
            {
                MoveComponent?.KnockBack(knockBackSrc, knockBackData, null, null);
            }
        }

        public override KnockBackData GetKnockBackData(EventParameters parameters)
        {
            return IsGroggy ? parameters.groggyKnockBackData : parameters.knockBackData;
        }

        #endregion
        

        public override void Die()
        {
            base.Die();
            IsRecognized = false;
            GameManager.Sound.Play("kill_dummy");
        }

    }
}