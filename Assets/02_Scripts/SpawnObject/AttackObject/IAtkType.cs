using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Apis
{
    public partial class AttackObject
    {
        public enum AttackTypeEnum
        {
            Normal,
            Once,
            Tick,
            Delay,
            Cd,
            OnlyFirst
        }

        public interface IAttackType
        {
            public void OnEnter();
            public void OnExit();
            public void OnInit();
            public void OnDisable();
            public void OnEnable();
        }

        public class NormalAttack : IAttackType // 기본적인 공격
        {
            private readonly AttackObject atk;

            public NormalAttack(AttackObject atk)
            {
                this.atk = atk;
            }

            public void OnEnter()
            {
                atk.AddEvent(EventType.OnTriggerEnter, Atk);
            }

            public void OnExit()
            {
                atk.RemoveEvent(EventType.OnTriggerEnter, Atk);
            }

            public void OnInit()
            {
            }

            public void OnDisable()
            {
            }

            public void OnEnable()
            {
            }

            private void Atk(EventParameters parameters)
            {
                if (parameters == null) return;

                if (ReferenceEquals(parameters.target, null)) return;
                if (!atk.CheckTarget(parameters.target)) return;

                Debug.Log(parameters.target);
                //info.user를 actor로 설정했다가 다시 취소함. 공격 성공시 user 위치에서 폭발 소환이 되지 않는 이유
                // actor는 대신에 info.master로 들어감

                atk.DoAttackInvoke(parameters);
                atk.DoAdditionalAtk(parameters);
            }
        }

        public class OnceAttack : IAttackType // 한번만 공격 (초기화 할때까지 같은 타겟 타격 X)
        {
            private readonly AttackObject atk;

            private List<IOnHit> _targets = new();

            public OnceAttack(AttackObject atk)
            {
                this.atk = atk;
            }

            private List<IOnHit> targets => _targets ??= new List<IOnHit>();

            public void OnEnter()
            {
                atk.AddEvent(EventType.OnTriggerEnter, Atk);
            }

            public void OnExit()
            {
                atk.RemoveEvent(EventType.OnTriggerEnter, Atk);
            }

            public void OnInit()
            {
                targets.Clear();
            }

            public void OnDisable()
            {
                targets.Clear();
            }

            public void OnEnable()
            {
                targets.Clear();
            }

            private void Atk(EventParameters parameters)
            {
                if (ReferenceEquals(parameters?.target, null)) return;
                if (!atk.CheckTarget(parameters.target)) return;

                if (targets.Contains(parameters.target)) return;

                atk.DoAttackInvoke(parameters);
                atk.DoAdditionalAtk(parameters);


                targets.Add(parameters.target);
            }
        }

        protected class TickAttack : IAttackType // 틱 공격 (일정 주기마다 계속 공격)
        {
            private readonly AttackObject atk;

            private float curTime;
            public float frequency;

            private readonly Dictionary<IOnHit, EventParameters> hitTargets;

            private readonly HashSet<IOnHit> targets;

            public TickAttack(AttackObject atk, float frequency)
            {
                this.atk = atk;
                this.frequency = frequency;
                targets = new HashSet<IOnHit>();
                hitTargets = new Dictionary<IOnHit, EventParameters>();
            }

            public void OnEnter()
            {
                atk.AddEvent(EventType.OnTriggerEnter, Enter);
                atk.AddEvent(EventType.OnTriggerExit, Exit);
            }

            public void OnExit()
            {
                atk.RemoveEvent(EventType.OnTriggerEnter, Enter);
                atk.RemoveEvent(EventType.OnTriggerExit, Exit);
            }

            public void OnInit()
            {
                targets.Clear();
                hitTargets.Clear();
                curTime = 0;
                atk.AddEvent(EventType.OnUpdate, Update);
            }

            public void OnDisable()
            {
                targets.Clear();
                hitTargets.Clear();
                atk.RemoveEvent(EventType.OnUpdate, Update);
            }

            public void OnEnable()
            {
                targets.Clear();
                hitTargets.Clear();
            }

            private void Update(EventParameters _)
            {
                curTime += Time.deltaTime;

                if (curTime > frequency)
                {
                    var keys = new List<IOnHit>(hitTargets.Keys);
                    foreach (var x in keys) atk.DoAttackInvoke(hitTargets[x]);
                    curTime = 0;
                }
            }

            private void Enter(EventParameters parameters)
            {
                if (ReferenceEquals(parameters?.target, null)) return;
                if (!atk.CheckTarget(parameters.target))
                    return;
                if (hitTargets.TryAdd(parameters.target, parameters) && !targets.Contains(parameters.target))
                {
                    atk.DoAttackInvoke(hitTargets[parameters.target], atk.firstDmg);
                    targets.Add(parameters.target);
                }
            }

            private void Exit(EventParameters parameters)
            {
                if (ReferenceEquals(parameters?.target, null)) return;
                hitTargets.Remove(parameters.target);
            }
        }

        protected class DelayContinuousAttack : IAttackType // 일정시간동안 접촉시 공격 (접촉해제시 초기화)
        {
            private Dictionary<IOnHit, Sequence> _delays = new();
            private Dictionary<IOnHit, EventParameters> _hitTargets = new();
            private readonly AttackObject atk;
            public float delayTime;
            private Sequence updateSequence;


            public DelayContinuousAttack(AttackObject atk, float delayTime)
            {
                this.atk = atk;
                this.delayTime = delayTime;
            }

            private Dictionary<IOnHit, EventParameters> hitTargets =>
                _hitTargets ??= new Dictionary<IOnHit, EventParameters>();

            private Dictionary<IOnHit, Sequence> delays => _delays ??= new Dictionary<IOnHit, Sequence>();

            public void OnEnter()
            {
                atk.AddEvent(EventType.OnTriggerEnter, Enter);
                atk.AddEvent(EventType.OnTriggerExit, Exit);
            }

            public void OnExit()
            {
                atk.RemoveEvent(EventType.OnTriggerEnter, Enter);
                atk.RemoveEvent(EventType.OnTriggerExit, Exit);
            }

            public void OnInit()
            {
                updateSequence = DOTween.Sequence();
                updateSequence.AppendCallback(() =>
                {
                    foreach (var x in hitTargets.Keys) atk.DoAttackInvoke(hitTargets[x]);
                });
                updateSequence.AppendInterval(Time.deltaTime);
                updateSequence.SetLoops(-1);
            }

            public void OnDisable()
            {
                updateSequence?.Kill();
            }

            public void OnEnable()
            {
            }

            private void Enter(EventParameters parameters)
            {
                if (ReferenceEquals(parameters?.target, null)) return;
                if (!atk.CheckTarget(parameters.target))
                    return;

                var seq = DOTween.Sequence();
                seq.SetDelay(delayTime);
                seq.AppendCallback(() => hitTargets.TryAdd(parameters.target, parameters));
                seq.AppendCallback(() => delays.Remove(parameters.target));
                delays.Add(parameters.target, seq);
            }

            private void Exit(EventParameters parameters)
            {
                if (ReferenceEquals(parameters?.target, null)) return;
                hitTargets.Remove(parameters.target);

                if (!delays.TryGetValue(parameters.target, out var value)) return;
                value.Kill();
                delays.Remove(parameters.target);
            }
        }

        public class CDAttack : IAttackType // 쿨타임 공격 (공격한 개체당 쿨타임 적용)
        {
            private readonly AttackObject atk;

            private List<IOnHit> _attackedTargets = new();
            private List<IOnHit> _stayingTargets = new();
            public float cd;

            public CDAttack(AttackObject atk, float cd)
            {
                this.atk = atk;
                this.cd = cd;
            }

            private List<IOnHit> attackedTargets => _attackedTargets ??= new List<IOnHit>();
            private List<IOnHit> stayingTargets => _stayingTargets ??= new List<IOnHit>();

            public void OnEnter()
            {
                atk.AddEvent(EventType.OnTriggerEnter, Atk);
                atk.AddEvent(EventType.OnTriggerEnter, Enter);
                atk.AddEvent(EventType.OnTriggerExit, Exit);
            }

            public void OnExit()
            {
                atk.RemoveEvent(EventType.OnTriggerEnter, Atk);
                atk.RemoveEvent(EventType.OnTriggerEnter, Enter);
                atk.RemoveEvent(EventType.OnTriggerExit, Exit);
            }

            public void OnInit()
            {
                attackedTargets.Clear();
                stayingTargets.Clear();
            }

            public void OnDisable()
            {
                attackedTargets.Clear();
                stayingTargets.Clear();
            }

            public void OnEnable()
            {
                attackedTargets.Clear();
                stayingTargets.Clear();
            }

            private void Enter(EventParameters parameters)
            {
                if (parameters?.target == null) return;

                if (stayingTargets.Contains(parameters.target)) return;

                stayingTargets.Add(parameters.target);
            }

            private void Exit(EventParameters parameters)
            {
                if (parameters?.target == null) return;
                stayingTargets.Remove(parameters.target);
            }

            private void Atk(EventParameters parameters)
            {
                if (parameters == null) return;

                if (ReferenceEquals(parameters.target, null)) return;
                if (!atk.CheckTarget(parameters.target)) return;

                if (attackedTargets.Contains(parameters.target)) return;

                attackedTargets.Add(parameters.target);

                GameManager.instance.StartCoroutineWrapper(Remove(parameters.target));
                atk.DoAttackInvoke(parameters);
                atk.DoAdditionalAtk(parameters);
            }

            private IEnumerator Remove(IOnHit target)
            {
                float curTime = 0;

                while (curTime < cd)
                {
                    curTime += Time.deltaTime;
                    yield return null;
                }

                attackedTargets.Remove(target);
                if (stayingTargets.Contains(target)) Atk(new EventParameters(atk?._eventUser, target));
            }
        }

        public class OnlyFirst : IAttackType // 처음 충돌한 개체 하나만 공격
        {
            private readonly AttackObject atk;

            private bool isAttacked;

            public OnlyFirst(AttackObject atk)
            {
                this.atk = atk;
            }

            public void OnEnter()
            {
                atk.AddEvent(EventType.OnTriggerEnter, Atk);
            }

            public void OnExit()
            {
                atk.RemoveEvent(EventType.OnTriggerEnter, Atk);
            }

            public void OnInit()
            {
                isAttacked = false;
            }

            public void OnDisable()
            {
                isAttacked = false;
            }

            public void OnEnable()
            {
                isAttacked = false;
            }

            private void Atk(EventParameters parameters)
            {
                if (parameters == null) return;

                if (ReferenceEquals(parameters.target, null)) return;
                if (!atk.CheckTarget(parameters.target)) return;

                if (isAttacked) return;

                isAttacked = true;

                atk.DoAttackInvoke(parameters);
                atk.DoAdditionalAtk(parameters);
            }
        }
    }
}