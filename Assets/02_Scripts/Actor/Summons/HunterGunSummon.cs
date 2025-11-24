using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public class HunterGunSummon : Summon
    {
        [HideInInspector]public float dmg;
        [HideInInspector]public float atkTime;
        [HideInInspector]public int atkCount;

        public override float Atk => Master.Atk;

        public enum States
        {
            Attack,Idle
        }
        class AttackState : IState<HunterGunSummon>
        {
            private HunterGunSummon actor;
            private float curTime;
            private EventParameters _atkParameters;
            private int count;
            private Actor target;
            
            public void OnEnter(HunterGunSummon t)
            {
                actor = t;
                _atkParameters = new(actor, actor.target);
                count = 0;
                target = t.target;
                if (target == null || target.IsDead)
                {
                    t.ChangeState(States.Idle);
                }

                isMoving = false;
                actor.StartCoroutine(MoveToTarget());
            }

            public void Update()
            {
                if (isMoving) return;
                
                curTime += Time.deltaTime;

                if (curTime > actor.atkTime)
                {
                    curTime = 0;
                    _atkParameters.atkData.atkStrategy = new AtkBase(actor, actor.dmg);
                    _atkParameters.atkData.attackType = Define.AttackType.Extra;
                    actor.Attack(_atkParameters);
                    count++;
                }
            }

            
            public void FixedUpdate()
            {
                if (CheckIdle() || isMoving)
                {
                    return;
                }
                
                actor.MoveTo(actor.target.Position);
            }

            bool CheckIdle()
            {
                if (!target || target.IsDead || count >= actor.atkCount)
                {
                    actor.ChangeState(States.Idle);
                    return true;
                }

                return false;
            }
            public void OnExit()
            {
                curTime = 0;
                target = null;
                actor.target = null;
            }

            private bool isMoving = false;
            IEnumerator MoveToTarget()
            {
                if (isMoving) yield break;

                isMoving = true;
                Vector2 pos = target.Position;
                float distance = Vector2.Distance(actor.Position, pos);

                while (distance > 0.1f)
                {
                    Vector2 dir = pos - (Vector2)actor.Position;
                    dir.Normalize();

                    actor.Rb.velocity = dir * actor.MoveSpeed / 100;
                    pos = actor.Position;
                    distance = Vector2.Distance(actor.Position, pos);
                    yield return new WaitForFixedUpdate();
                }

                isMoving = false;
            }
        }

        class IdleState : IState<HunterGunSummon>
        {
            private HunterGunSummon actor;
            public void OnEnter(HunterGunSummon t)
            {
                actor = t;
                actor.StartCoroutine(actor.ReturnToPlayer());
            }

            public void Update()
            {
                if (!actor.isReturning && actor.target && !actor.target.IsDead)
                {
                    actor.ChangeState(States.Attack);
                }
            }

            public void FixedUpdate()
            {
                actor.MoveTo((Vector2)actor.Master.Position + new Vector2(-(int)actor.Master.Direction,1));
            }

            public void OnExit()
            {
            }
        }

        private Dictionary<States, IState<HunterGunSummon>> _stateDict;
        private Dictionary<States, IState<HunterGunSummon>> stateDict => _stateDict ??= new();

        private Actor target;
        private StateMachine<HunterGunSummon> _stateMachine;

        public override void IdleOn()
        {
        }

        public override void AttackOn()
        {
        }

        public override void AttackOff()
        {
        }

        protected override void Awake()
        {
            base.Awake();
            stateDict.Add(States.Attack,new AttackState());
            stateDict.Add(States.Idle,new IdleState());
        }

        protected override void Start()
        {
            base.Start();
            _stateMachine = new StateMachine<HunterGunSummon>(this, stateDict[States.Idle]);
        }

        public override float OnHit(EventParameters parameters)
        {
            return 0;
        }

        public override void SetMaster(Actor master)
        {
            base.SetMaster(master);
            Master.AddEvent(EventType.OnBasicAttack, AddTarget);
        }

        public override void OnUnSummon()
        {
            Master.RemoveEvent(EventType.OnBasicAttack, AddTarget);
        }

        protected override void Update()
        {
            base.Update();
            if (isReturning) return;
            _stateMachine.Update();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isReturning) return;
            _stateMachine.FixedUpdate();
        }

        void AddTarget(EventParameters parameters)
        {
            if (parameters?.target is Actor t )
            {
                target = t;
            }
        }

        public void ChangeState(States state)
        {
            _stateMachine.SetState(stateDict[state]);
        }

        [HideInInspector] public bool isReturning;

        IEnumerator ReturnToPlayer()
        {
            if (isReturning) yield break;
            
            Vector2 pos = (Vector2)Master.Position + new Vector2(-(int)Master.Direction, 1);
            float distance = Vector2.Distance(Position, pos);
            isReturning = true;

            while (distance > 0.1f)
            {
                Vector2 dir = pos - Rb.position;
                dir.Normalize();

                Rb.velocity = dir * MoveSpeed / 100;
                pos = (Vector2)Master.Position + new Vector2(-(int)Master.Direction, 1);
                distance = Vector2.Distance(Position, pos);
                yield return new WaitForFixedUpdate();
            }

            isReturning = false;
        }

        void MoveTo(Vector2 pos)
        {
            float distance = Vector2.Distance(Position, pos);

            if (distance > 0.2f)
            {
                Vector2 dir = pos - Rb.position;
                dir.Normalize();

                Rb.velocity = dir * MoveSpeed / 100;
            }
            else
            {
                Rb.position = pos;
            }
        }
    }
}