using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    public class StickTargets : ProjectileExtension
    {
        [HideIf("isAvailable")]
        [InfoBox("투사체가 벽을 뚫지 않도록 설정해주세요.\n설정하지 않으면 실행되지 않습니다", InfoMessageType.Error)]
        [DisplayAsString]
        public string Error = "";

        [InfoBox("타겟이 붙었을 때, 이동 및 공격을 할 수 있는지 여부")] [LabelText("타겟 이동여부")]
        public bool isMove;

        [LabelText("타겟 공격여부")] public bool isAtk;

        private Vector2 lastPos;

        [Space(20)] private List<IOnHit> targets = new();

        private bool isAvailable =>
            (projectile.projectileInfo?.wallConflictType ?? projectile.wallConflictType)
            is not (ProjectileConflictType.None or ProjectileConflictType.Penetrate);

        private void Start()
        {
            if (projectile.wallConflictType is ProjectileConflictType.None or ProjectileConflictType.Penetrate)
            {
                Debug.Log("벽을 통과하는 투사체는 stickTarget을 사용할 수 없습니다. (버그방지)");
                return;
            }

            projectile.AddEvent(EventType.OnAttackSuccess, AddTarget);
            projectile.AddEvent(EventType.OnDestroy, RemoveTargets);
            projectile.AddEvent(EventType.OnInit, RemoveTargets);
            projectile.AddEvent(EventType.OnTriggerExit, RemoveTarget);
            targets = new List<IOnHit>();
        }

        private void FixedUpdate()
        {
            if (!isAvailable) return;
            if (projectile?.Fired ?? false)
            {
                var distance = transform.position.x - lastPos.x;

                var temp = targets.ToList();
                temp.ForEach(x =>
                {
                    if (x.IsAffectedByCC)
                    {
                        if (x is Actor actor)
                            actor.Rb.MovePosition(actor.Rb.position + Vector2.right * distance);
                        else
                            x.transform.Translate(Vector2.right * distance);
                    }
                });
                lastPos = transform.position;
            }
        }

        private void AddTarget(EventParameters parameters)
        {
            if (!isAvailable) return;
            if (parameters?.target != null)
            {
                targets.Add(parameters.target);
                if (parameters.target is Actor { IsAffectedByCC: true } target)
                {
                    target.IdleOn();
                    if (!isMove && target is IMovable mover) mover.MoveCCOn();

                    if (!isAtk) target.AttackOff();
                }
            }
        }

        private void RemoveTargets(EventParameters _)
        {
            if (!isAvailable) return;
            targets.ForEach(x =>
            {
                if (x is Actor { IsAffectedByCC: true } target)
                {
                    if (!isMove && target is IMovable mover) mover.MoveCCOff();

                    if (!isAtk) target.AttackOn();
                }
            });
            targets.Clear();
        }

        private void RemoveTarget(EventParameters parameters)
        {
            if (parameters?.target is Actor { IsAffectedByCC: true } target)
            {
                if (!isMove && target is IMovable mover) mover.MoveCCOff();

                if (!isAtk) target.AttackOn();

                targets.Remove(parameters.target);
            }
        }
    }
}