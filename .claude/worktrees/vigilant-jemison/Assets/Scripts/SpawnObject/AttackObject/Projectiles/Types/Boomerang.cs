using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Apis
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Boomerang : Projectile
    {
        [TabGroup("부메랑 설정")] [LabelText("돌아오는 속도")]
        public float speed2;

        [FormerlySerializedAs("stopUse")] [TabGroup("부메랑 설정")] [LabelText("제한시간 사용여부")]
        public bool timeUse;

        [ShowIf("timeUse")] [TabGroup("부메랑 설정")] [LabelText("제한시간")]
        public float time;

        private UnityEvent<Boomerang> _onStop = new();
        protected bool isStop;
        public UnityEvent<Boomerang> OnStop => _onStop ??= new UnityEvent<Boomerang>();

        protected override void FixedUpdate()
        {
            if (isReturn) return;

            base.FixedUpdate();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StopAllCoroutines();
        }

        public override void Init(AttackObjectInfo atkObjectInfo)
        {
            base.Init(atkObjectInfo);
            isReturn = false;
            isStop = false;
        }

        public virtual void Stop()
        {
            fired = false;
            rigid.linearVelocity = Vector2.zero;
            if (timeUse)
            {
                var stopSeq = DOTween.Sequence();
                stopSeq.SetDelay(time);
                stopSeq.AppendCallback(ReturnToActor);
            }

            OnStop.Invoke(this);
            isStop = true;
            Collider.enabled = false;
        }

        public override void Fire(bool rotateWithPlayerX = true)
        {
            base.Fire(rotateWithPlayerX);
            isReturn = false;
            isStop = false;
        }

        protected virtual void FirstAttackInvoke(EventParameters parameters)
        {
            if (isAtk) Attack(parameters);
        }

        protected virtual void ReturnAttackInvoke(EventParameters parameters)
        {
            if (isAtk) Attack(parameters);
        }

        protected override void AttackInvoke(EventParameters parameters)
        {
            if (!isReturn)
                FirstAttackInvoke(parameters);
            else
                ReturnAttackInvoke(parameters);

            if (!isStop) OnObjectConflicted(targetConflictType, targetLayer);
            ExecuteEvent(EventType.OnAttack, parameters);
        }

        public override void Destroy()
        {
            if (isStop)
                base.Destroy();
            else
                Stop();
        }

        public virtual void ReturnToActor()
        {
            StartCoroutine(ReturnToActorCoroutine());
        }

        protected override void OnObjectConflicted(ProjectileConflictType conflictType, LayerMask layerMasks)
        {
            if (isStop) return;

            base.OnObjectConflicted(conflictType, layerMasks);
        }

        private IEnumerator ReturnToActorCoroutine()
        {
            if (!isReturn)
            {
                Collider.enabled = true;
                AttackType.OnEnable();
                fired = true;
                isReturn = true;
                var distance = Vector2.Distance(_attacker.Position, transform.position);
                var pos = rigid.position;

                var dir = ((Vector2)_attacker.Position - pos).normalized;
                rigid.linearVelocity = dir * speed2;

                while (distance > 0.1f)
                {
                    distance = Vector2.Distance(_attacker.Position, rigid.position);
                    pos = rigid.position;

                    var mag = rigid.linearVelocity.magnitude;
                    dir = ((Vector2)_attacker.Position - pos).normalized;
                    rigid.linearVelocity = dir * mag;
                    rigid.linearVelocity += GetAccelerationVector();
                    var angle = Mathf.Atan2(rigid.linearVelocity.y, rigid.linearVelocity.x) * Mathf.Rad2Deg;
                    ThisTrans.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    yield return new WaitForFixedUpdate();
                }

                Destroy();
                isReturn = false;
            }
        }
    }
}