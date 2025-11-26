using System;
using Apis.BehaviourTreeTool;
using Apis;
using System.Collections.Generic;
using UnityEngine;
using Apis.DataType;
using UnityEngine.Events;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using Default;
using Save.Schema;
using Spine.Unity;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Apis
{
    public abstract class BossMonster : Monster
    { 
        #region Enums
        public enum BossState
        {
            Wait = 0, Prepare, Move, Attack, Delay, Groggy, Transform, Down, Summon1, Summon2,Climb,Idle,Dash,
        }
        public enum BossPhase
        {
            Phase1 = 1, Phase2
        }
        #endregion
        #region 상태 클래스

        class IdleState : IState<BossMonster>
        {
            public void OnEnter(BossMonster t)
            {
                t.state = BossState.Idle;
                t.HitCollider.enabled = true;
            }

            public void Update()
            {
            }

            public void FixedUpdate()
            {
            }

            public void OnExit()
            {
            }
        }

        class DashState : IState<BossMonster>
        {
            public void OnEnter(BossMonster t)
            {
                t.state = BossState.Dash;
            }

            public void Update()
            {
            }

            public void FixedUpdate()
            {
            }

            public void OnExit()
            {
            }
        }
        class MoveState : IState<BossMonster>
        {
            BossMonster boss;
            public void FixedUpdate()
            {                          
            }

            public void OnEnter(BossMonster t)
            {
                boss = t;
                t.state = BossState.Move;
                boss.ActorMovement.ResetGravity();
                boss.HitCollider.enabled = true;
            }

            public void OnExit()
            {
                boss.ActorMovement.StopWithFall();
            }

            public void Update()
            {
               
            }            
        }
        class WaitState : IState<BossMonster>
        {
            BossMonster boss;
            private Guid guid;
            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                boss = t;
                t.state = BossState.Wait;
                t.ActorMovement.StopWithFall();
                boss.ActorMovement.ResetGravity();
                guid = t.AddInvincibility();
            }

            public void OnExit()
            {
                boss.RemoveInvincibility(guid);
            }

            public void Update()
            {
            }
        }

        class PrepareState : IState<BossMonster>
        {
            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                t.state = BossState.Prepare;
                t.ActorMovement.ResetGravity();
                t.ActorMovement.StopWithFall();
            }

            public void OnExit()
            {
            }

            public void Update()
            {
            }
        }
        class AttackState : IState<BossMonster>
        {
            private BossMonster _boss;
            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                _boss = t;
                t.ActorMovement.Stop();
                t.state = BossState.Attack;
                t.animator.ResetTrigger("AttackStart");
                t.animator.ResetTrigger("AttackEnd");
            }

            public void OnExit()
            {
            }

            public void Update()
            {
            }
            
        }
        class DelayState : IState<BossMonster>
        {
            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                t.state = BossState.Delay;
                t.ActorMovement.ResetGravity();
                t.ActorMovement.StopWithFall();
            }

            public void OnExit()
            {
            }

            public void Update()
            {
                
            }
        }

        class GroggyState : IState<BossMonster>
        {
            private BossMonster boss;
            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                boss = t;
                t.state = BossState.Groggy;
                t.ActorMovement.StopWithFall();
                t.IsGroggy = true;
                boss.animator.ResetTrigger("GroggyEnd");
                boss.ActorMovement.ResetGravity();
            }

            public void OnExit()
            {
                boss.IsGroggy = false;
                boss.animator.SetTrigger("GroggyEnd");
            }

            public void Update()
            {
            }
        }
        class TransformState : IState<BossMonster>
        {
            BossMonster boss;
            private Guid guid;
            public void FixedUpdate()
            {
                guid = boss.AddInvincibility();
            }

            public void OnEnter(BossMonster t)
            {
                boss = t;
                t.state = BossState.Transform;
                boss.ActorMovement.StopWithFall();
                boss.ActorMovement.ResetGravity();
                t.AddInvincibility();
            }

            public void OnExit()
            {
                boss.RemoveInvincibility(guid);
            }

            public void Update()
            {
            }
        }
        class DownState : IState<BossMonster>
        {
            private BossMonster boss;
            public void FixedUpdate()
            {
            }

            public void OnEnter(BossMonster t)
            {
                boss = t;
                t.state = BossState.Down;
                t.ActorMovement.ResetGravity();
                t.Rb.DOKill();
                t.AddInvincibility();
                t.HitCollider.enabled = false;
            }

            public void OnExit()
            {
                boss.HitCollider.enabled = true;
            }

            public void Update()
            {
            }
        }

        #endregion

        protected StateMachine<BossMonster> stateMachine;

        protected BossState state;
        public BossState State => state;
        [HideInInspector] public BossPhase phase;
        
        protected readonly IDictionary<BossState, IState<BossMonster>> stateDict = new Dictionary<BossState, IState<BossMonster>>();      

        [SerializeField] EActorDirection startDirection;
        
        readonly List<(int rad,Color color)> radiusList = new();

        protected List<AttackObject> colliderList;

        [HideInInspector] public Action OnTeleportStart;
        [HideInInspector] public Action OnTeleportEnd;

        private Collider2D _hitCollider;
        public override Collider2D HitCollider => _hitCollider ??= transform.Find("HitCollider").GetComponent<Collider2D>();

        [TabGroup("기획쪽 수정 변수들/group1", "기본 스탯")]
        [LabelText("그로기 최대 게이지")] public float MaxGroggyGauge;
        [TabGroup("기획쪽 수정 변수들/group1", "기본 스탯")]
        [LabelText("초당 그로기 회복량")]
        public float GroggyRecovery;

        [HideInInspector] public BehaviourTreeRunner treeRunner;

        private GameObject _bossAttacks;
        public int cutSceneId;
        
        protected GameObject BossAttacks
        {
            get
            {
                if (_bossAttacks == null)
                {
                    _bossAttacks = new GameObject("BossAttacks");
                }

                return _bossAttacks;
            }
        }

        public override bool IsAffectedByCC => false;
        protected Dictionary<int, IBossAttackPattern> attackPatterns;

        [Title("이벤트")]
        public UnityEvent OnBattleStart = new();
        public UnityEvent OnDeath = new();
        public UnityEvent OnTransformStart = new();
        public UnityEvent OnTransformEnd = new();
        public bool isTest;
        public int taskIndex;
        
        protected override void Awake()
        {
            base.Awake();
            
            animator = Utils.GetComponentInParentAndChild<Animator>(gameObject);
            treeRunner = GetComponent<BehaviourTreeRunner>();

            stateDict.Add(BossState.Wait, new WaitState());
            stateDict.Add(BossState.Prepare, new PrepareState());
            stateDict.Add(BossState.Delay, new DelayState());
            stateDict.Add(BossState.Move, new MoveState());
            stateDict.Add(BossState.Attack, new AttackState());
            stateDict.Add(BossState.Groggy, new GroggyState());
            stateDict.Add(BossState.Transform, new TransformState());
            stateDict.Add(BossState.Down, new DownState());
            stateDict.Add(BossState.Idle, new IdleState());
            stateDict.Add(BossState.Dash, new DashState());
            
            stateMachine = new(this, stateDict[BossState.Wait]);
            Direction = startDirection;
            colliderList = GetComponentsInChildren<AttackObject>(true).ToList();
            BehaviourTree.Traverse(GetComponent<BehaviourTreeRunner>().tree.rootNode, (a) =>
            {
                if (a is IfPlayerDistance pd)
                {
                    radiusList.Add((pd.distance, pd.color));
                }
            });
            SetState(isTest ? BossState.Move : BossState.Wait);
            SetAttackPatterns();
        }

        protected override void Start()
        {
            Debug.Log(name +"Started");
            base.Start();
        }

        protected abstract void SetAttackPatterns();

        [HideInInspector] public int currentAtkPattern;

        public void StartAtkPattern(int index)
        {
            Rb.DOKill();
            animator.ResetTrigger("AttackStart");
            animator.ResetTrigger("AttackEnd");
            if (attackPatterns.TryGetValue(index, out var value))
            {
                currentAtkPattern = index;
                value.OnPatternEnter();
            }
        }
        public void DoAttack(string str) // 애니메이션에서 사용
        {
            animator.ResetTrigger("AttackStart");
            animator.ResetTrigger("AttackEnd");
            string[] strs = str.Split(',');
            int[] ints = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                ints[i] = int.Parse(strs[i]);
            }
            if (attackPatterns.TryGetValue(ints[0], out var value))
            {
                value.Attack(ints[1]);
            }

        }

        public void SetColliders(string str) // 애니메이션에서 사용
        {
            string[] strs = str.Split(',');
            int[] ints = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                ints[i] = int.Parse(strs[i]);
            }
            if (attackPatterns.TryGetValue(ints[0], out var value))
            {
                value.SetCollider(ints[1]);
            }
        }
        public virtual void CancelAttack()
        {
            if (attackPatterns.TryGetValue(currentAtkPattern, out var value))
            {
                value.Cancel();
            }
        }

        public void EndAttack(int pattern)
        {
            if (attackPatterns.TryGetValue(pattern, out var value))
            {
                value.End();
            }
        }

        public override void IdleOn()
        {
            base.IdleOn();

            if (!IsRecognized) return;
            CancelAttack();
            SetState(BossState.Move);
        }

        public override void EndStun()
        {
            base.EndStun();
            animator.SetTrigger("GroggyEnd");
        }

        public void SetState(BossState state)
        {
            animator.ResetTrigger("ChangeState");

            stateMachine.SetState(stateDict[state]);
            animator.SetInteger("State", (int)state);
            animator.SetTrigger("ChangeState");

            this.state = state;
        }
        
        protected override void Update()
        {
            base.Update();
            stateMachine.Update();           
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            stateMachine.FixedUpdate();
        }

        public override void StartStun(IEventUser actor, float duration)
        {
            base.StartStun(actor, duration);
            SetState(BossState.Groggy);
        }
        public override void OnRecognized()
        {
            base.OnRecognized();
            treeRunner.StartRunningTree();
            WhenRecognized();
        }

        protected virtual void WhenRecognized()
        {
            Sequence seq = DOTween.Sequence();
            seq.SetDelay(2);
            seq.AppendCallback(() =>
            {
                if (isTest) return;
                SetState(BossState.Move);
            });
            seq.AppendCallback(() =>
            {
                base.OnRecognized();
                OnBattleStart.Invoke();
            });
            seq.Play();
        }
        public void FlipToPlayer()
        {
            if (GameManager.instance.ControllingEntity == null) return;

            float dirX = GameManager.instance.ControllingEntity.Position.x - Position.x;
            SetDirection(dirX < 0 ? EActorDirection.Left : EActorDirection.Right );
        }

        public delegate Vector2 SetPosition();
        public Sequence Teleport(float duration, SetPosition WhenTeleport)
        {
            Rb.DOKill();
            OnTeleportStart?.Invoke();
            Hide();
            Guid guid = AddInvincibility();
            Sequence seq = DOTween.Sequence();
            seq.SetDelay(duration);
            seq.AppendCallback(() =>
            {
                // 발바닥 기준으로 맞추기 위해서 위치를 transform.position으로 맞춤
                transform.position = WhenTeleport.Invoke();
                RemoveInvincibility(guid);
                Appear();
                OnTeleportEnd?.Invoke();
            });
            return seq;
        }
        
        public Sequence Teleport(Vector2 position, float duration)
        {
            Rb.DOKill();
            OnTeleportStart?.Invoke();
            Hide();
            Guid guid = AddInvincibility();
            Sequence seq = DOTween.Sequence();
            float distance = position.x - transform.position.x;
            var ray = Physics2D.Raycast(transform.position, distance > 0 ? Vector2.right : Vector2.left,
                Mathf.Abs(distance), LayerMasks.Wall);

            if (ray.collider != null)
            {
                position.x = transform.position.x + (distance > 0 ? ray.distance : -ray.distance);
            }
            
            seq.SetDelay(duration);
            seq.AppendCallback(() =>
            {
                // 발바닥 기준으로 맞추기 위해서 위치를 transform.position으로 맞춤
                transform.position = position;
                RemoveInvincibility(guid);
                Appear();
                OnTeleportEnd?.Invoke();
            });
            
            return seq;
        }
        public Sequence Teleport(string posName, float duration)
        {
            Rb.DOKill();

            OnTeleportStart?.Invoke();
            Hide();
            Guid guid = AddInvincibility();
            Transform obj = GameObject.Find(posName).transform;
            Sequence seq = DOTween.Sequence();
            seq.SetDelay(duration);
            seq.AppendCallback(() =>
            {
                transform.position = obj.position;
                RemoveInvincibility(guid);
                Appear();
                OnTeleportEnd?.Invoke();
            });
            
            return seq;
        }
        
        public void Teleport(string posName)
        {
            Rb.DOKill();
            OnTeleportStart?.Invoke();
            Transform obj = GameObject.Find(posName).transform;
            Position = obj.position;
            OnTeleportEnd?.Invoke();
        }

        [Button]
        public void TestAttack(int index,int type)
        {
            if (state != BossState.Attack)
            {
                SetState(BossState.Attack);
            }
            animator.ResetTrigger("AttackStart");
            animator.ResetTrigger("AttackEnd");
            animator.SetInteger("Attack",index);
            animator.SetInteger("AttackType",type);
            animator.SetBool("IsAttackEnd",true);
            int idx = type > 0 ? index * 100 + type : index;
            StartAtkPattern(idx);
        }
        
        public Tween MoveToPlayer(float meleeDistance,float minDistance,float maxDistance,float duration,Ease ease)
        {
            Rb.DOKill();
            
            float playerX = GameManager.instance.ControllingEntity.Position.x;
            float x = Position.x;

            float moveDist;
            if (x < playerX && Direction == EActorDirection.Right || x > playerX && Direction == EActorDirection.Left)
            {
                float endX = x > playerX ? playerX + meleeDistance : playerX - meleeDistance;
                moveDist = Mathf.Clamp(Mathf.Abs(endX - x), minDistance, maxDistance);
                moveDist *= DirectionScale;
            }
            else
            {
                moveDist = minDistance * DirectionScale;
            }

            Tween tween = Rb.DOMoveX(moveDist, duration).SetRelative().SetEase(ease);
            tween.KillWhenBoxCast(this, 0.5f, Vector2.right * DirectionScale, new Vector2(0.2f, 1), LayerMasks.Wall);
           
            return tween;
        }

        public (Tween,Tween) JumpToPlayer(float meleeDistance, float minDistance, float maxDistance, float jumpHeight, float duration)
        {
            Rb.DOKill();

            float playerX = GameManager.instance.ControllingEntity.Position.x;
            float x = Position.x;

            float moveDist;
            if (x < playerX && Direction == EActorDirection.Right || x > playerX && Direction == EActorDirection.Left)
            {
                float endX = x > playerX ? playerX + meleeDistance : playerX - meleeDistance;
                moveDist = Mathf.Clamp(Mathf.Abs(endX - x), minDistance, maxDistance);
            }
            else
            {
                moveDist = minDistance;
            }

            (Tween x,Tween y) tween = ActorMovement.DoJumpTween(duration, jumpHeight, moveDist, false);
            tween.x.KillWhenBoxCast(this,0.5f, Vector2.right * DirectionScale, new Vector2(0.2f, 1), LayerMasks.Wall);

            return tween;
        }

        public (Tween,Tween) JumpToPlayer(float meleeDistance, float minDistance, float maxDistance, float jumpHeight,
            float endHeight, float duration)
        {
            Rb.DOKill();

            float playerX = GameManager.instance.ControllingEntity.Position.x;
            float x = Position.x;

            float y = endHeight;
            
            float moveDist;
            if ((x < playerX && Direction == EActorDirection.Right || x > playerX && Direction == EActorDirection.Left) && Mathf.Abs(x - playerX) > meleeDistance)
            {
                float endX = x > playerX ? playerX + meleeDistance : playerX - meleeDistance;
                moveDist = Mathf.Clamp(Mathf.Abs(endX - x), minDistance, maxDistance);
            }
            else
            {
                moveDist = minDistance;
            }

            (Tween x,Tween y) tween = ActorMovement.DoJumpTween(duration,jumpHeight,moveDist,y,false);

            if (moveDist > 0)
            {
                tween.x.KillWhenBoxCast(this,0.5f, Vector2.right * DirectionScale, new Vector2(0.2f, 1), LayerMasks.Wall);
            }
            else
            {
                tween.x.KillWhenBoxCast(this,0.5f, Vector2.left * DirectionScale, new Vector2(0.2f, 1), LayerMasks.Wall);
            }
            return tween;
        }
        [Button]
        public void MakeGroggy()
        {
            StartStun(GameManager.instance.Player,1f);
            CancelAttack();
        }
        public override void Die()
        {
            base.Die();
            treeRunner.tree.CancelCurrentNode();

            if (state != BossState.Down)
            {
                SetState(BossState.Down);
            }
            
            BossAttacks.SetActive(false);
            OnDeath?.Invoke();
        }
    }  
}