using System;
using System.Collections.Generic;
using System.Linq;
using Apis.BehaviourTreeTool;
using Default;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
#endif

namespace Apis
{
    public abstract class BossMonster : Monster
    {
        public delegate Vector2 SetPosition();

        [HideInInspector] public BossPhase phase;

        [SerializeField] private EActorDirection startDirection;

        [HideInInspector] public BehaviourTreeRunner treeRunner;

        [Title("이벤트")] public UnityEvent OnBattleStart = new();

        public UnityEvent OnDeath = new();
        public UnityEvent OnTransformStart = new();
        public UnityEvent OnTransformEnd = new();
        public bool isTest;
        public int taskIndex;

        [HideInInspector] public int currentAtkPattern;

        protected readonly IDictionary<BossState, IState<BossMonster>> stateDict =
            new Dictionary<BossState, IState<BossMonster>>();

        private GameObject _bossAttacks;

        private Collider2D _hitCollider;
        protected Dictionary<int, IBossAttackPattern> attackPatterns;

        protected List<AttackObject> colliderList;
        public Action OnTeleportEnd;

        public Action OnTeleportStart;

        protected BossState state;

        protected StateMachine<BossMonster> stateMachine;
        public BossState State => state;

        public override Collider2D HitCollider =>
            _hitCollider ??= transform.Find("HitCollider").GetComponent<Collider2D>();

        protected GameObject BossAttacks
        {
            get
            {
                if (_bossAttacks == null) _bossAttacks = new GameObject("BossAttacks");

                return _bossAttacks;
            }
        }

        public override bool IsAffectedByCC => false;

        protected override void Awake()
        {
            base.Awake();

            animator = Utils.GetComponentInParentAndChild<Animator>(gameObject);
            treeRunner = GetComponent<BehaviourTreeRunner>();

            stateDict.Add(BossState.Wait, new WaitState());
            stateDict.Add(BossState.Delay, new DelayState());
            stateDict.Add(BossState.Move, new MoveState());
            stateDict.Add(BossState.Attack, new AttackState());
            stateDict.Add(BossState.Down, new DownState());
            stateDict.Add(BossState.Idle, new IdleState());
            stateDict.Add(BossState.Dash, new DashState());

            stateMachine = new StateMachine<BossMonster>(this, stateDict[BossState.Wait]);
            Direction = startDirection;
            colliderList = GetComponentsInChildren<AttackObject>(true).ToList();
            
            SetState(isTest ? BossState.Move : BossState.Wait);
            SetAttackPatterns();
        }

        protected override void Start()
        {
            Debug.Log(name + "Started");
            base.Start();
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

        protected abstract void SetAttackPatterns();

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
            var strs = str.Split(',');
            var ints = new int[strs.Length];
            for (var i = 0; i < strs.Length; i++) ints[i] = int.Parse(strs[i]);
            if (attackPatterns.TryGetValue(ints[0], out var value)) value.Attack(ints[1]);
        }

        public void SetColliders(string str) // 애니메이션에서 사용
        {
            var strs = str.Split(',');
            var ints = new int[strs.Length];
            for (var i = 0; i < strs.Length; i++) ints[i] = int.Parse(strs[i]);
            if (attackPatterns.TryGetValue(ints[0], out var value)) value.SetCollider(ints[1]);
        }

        public virtual void CancelAttack()
        {
            if (attackPatterns.TryGetValue(currentAtkPattern, out var value)) value.Cancel();
        }

        public void EndAttack(int pattern)
        {
            if (attackPatterns.TryGetValue(pattern, out var value)) value.End();
        }

        public override void IdleOn()
        {
            base.IdleOn();

            if (!IsRecognized) return;
            CancelAttack();
            SetState(BossState.Move);
        }

        public void SetState(BossState state)
        {
            animator.ResetTrigger("ChangeState");

            stateMachine.SetState(stateDict[state]);
            animator.SetInteger("State", (int)state);
            animator.SetTrigger("ChangeState");

            this.state = state;
        }

        public override void OnRecognized()
        {
            base.OnRecognized();
            treeRunner.StartRunningTree();
            WhenRecognized();
        }

        protected virtual void WhenRecognized()
        {
            var seq = DOTween.Sequence();
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

            var dirX = GameManager.instance.ControllingEntity.Position.x - Position.x;
            SetDirection(dirX < 0 ? EActorDirection.Left : EActorDirection.Right);
        }

        public Sequence Teleport(float duration, SetPosition WhenTeleport)
        {
            Rb.DOKill();
            OnTeleportStart?.Invoke();
            Hide();
            var guid = AddInvincibility();
            var seq = DOTween.Sequence();
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
            var guid = AddInvincibility();
            var seq = DOTween.Sequence();
            var distance = position.x - transform.position.x;
            var ray = Physics2D.Raycast(transform.position, distance > 0 ? Vector2.right : Vector2.left,
                Mathf.Abs(distance), LayerMasks.Wall);

            if (ray.collider != null) position.x = transform.position.x + (distance > 0 ? ray.distance : -ray.distance);

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
            var guid = AddInvincibility();
            var obj = GameObject.Find(posName).transform;
            var seq = DOTween.Sequence();
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
            var obj = GameObject.Find(posName).transform;
            Position = obj.position;
            OnTeleportEnd?.Invoke();
        }

        [Button]
        public void TestAttack(int index, int type)
        {
            if (state != BossState.Attack) SetState(BossState.Attack);
            animator.ResetTrigger("AttackStart");
            animator.ResetTrigger("AttackEnd");
            animator.SetInteger("Attack", index);
            animator.SetInteger("AttackType", type);
            animator.SetBool("IsAttackEnd", true);
            var idx = type > 0 ? index * 100 + type : index;
            StartAtkPattern(idx);
        }

        public Tween MoveToPlayer(float meleeDistance,float minDistance,float maxDistance,float duration,Ease ease)
        {
            ActorMovement.Stop();
            
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

            var tween = Rb.DOMoveX(Rb.position.x + moveDist,duration)
            .SetEase(ease).SetUpdate(UpdateType.Fixed);
            tween.KillWhenBoxCast(Rb, new Vector2(0.2f, 1), LayerMasks.Wall);
           
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

            Vector2 endPos = (Vector2)transform.position + Vector2.right * (moveDist * DirectionScale);
            (Tween x,Tween y) tween = ActorMovement.DoJumpTween(duration, jumpHeight, 
                endPos, LayerMasks.Wall);

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

            Vector2 endPos = (Vector2)transform.position + Vector2.right * (moveDist * DirectionScale);
            
            (Tween x, Tween y) tween = ActorMovement.DoJumpTween(duration, jumpHeight,
                endPos, LayerMasks.Wall);
            
            
            return tween;
        }
        
        public override void Die()
        {
            base.Die();
            treeRunner.tree.CancelCurrentNode();

            if (state != BossState.Down) SetState(BossState.Down);

            BossAttacks.SetActive(false);
            OnDeath?.Invoke();
        }

        #region Enums

        public enum BossState
        {
            Wait = 0,
            Move,
            Attack,
            Delay,
            Transform,
            Down,
            Idle,
            Dash
        }

        public enum BossPhase
        {
            Phase1 = 1,
            Phase2
        }

        #endregion

        #region 상태 클래스

        private class IdleState : IState<BossMonster>
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

        private class DashState : IState<BossMonster>
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

        private class MoveState : IState<BossMonster>
        {
            private BossMonster boss;

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

        private class WaitState : IState<BossMonster>
        {
            private BossMonster boss;
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

        private class AttackState : IState<BossMonster>
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

        private class DelayState : IState<BossMonster>
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

        private class DownState : IState<BossMonster>
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
    }
}