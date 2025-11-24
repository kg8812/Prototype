using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class Grab : AttackObject
    {
        private List<Actor> targets;

        protected override void Awake()
        {
            base.Awake();
            targets = new();
        }

        protected override void AttackInvoke(EventParameters parameters)
        {
            base.AttackInvoke(parameters);
            if (parameters?.target is Actor target && !targets.Contains(target) && target.IsAffectedByCC)
            {
                targets.Add(target);
            }
        }

        public override void Init(AttackObjectInfo atkObjectInfo)
        {
            base.Init(atkObjectInfo);

            targets.Clear();
        }

        private Tween tween;

        public override void Destroy()
        {
            base.Destroy();
            targets.ForEach(x =>
            {
                if (x is IMovable mover)
                {
                    mover.MoveCCOff();
                }
            });
            targets.Clear();
            tween?.Kill();
        }

        public void MoveToPos(Vector2 pos, float time, UnityAction<AttackObject> OnEnd)
        {
            tween = transform.DOMoveX(pos.x, time).SetEase(Ease.OutSine).SetAutoKill(true);
            Vector2 lastPos = transform.position;

            tween.onUpdate += () =>
            {
                float distance = transform.position.x - lastPos.x;
                for (int i = 0; i < targets.Count; i++)
                {
                    targets[i].transform.Translate(Vector2.right * distance);
                }

                lastPos = transform.position;
            };
            tween.onKill += () => { OnEnd?.Invoke(this); };
        }
    }
}