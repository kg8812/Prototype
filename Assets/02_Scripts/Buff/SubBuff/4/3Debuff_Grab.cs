using DG.Tweening;
using UnityEngine;

namespace Apis
{
    public class Debuff_Grab : Debuff_CC
    {
        public Debuff_Grab(Buff effect) : base(effect)
        {          
        }

        public override SubBuffType Type => SubBuffType.Debuff_Grab;


        public override void OnAdd()
        {
            base.OnAdd();
            
            Vector3 pos = target.TryGetComponent(out Actor act) ? act.Position : target.transform.position;

            actor.Rb.DOMove(pos, duration).SetEase(Ease.Linear);
            if (actor is IMovable mover)
            {
                mover.MoveCCOn();
            }
            actor.AttackOff();

        }

        public override void OnRemove()
        {
            base.OnRemove();

            if (actor is IMovable mover)
            {
                mover.MoveCCOff();
            }
            actor.AttackOn();
        }

        public override void Update()
        {
            base.Update();

            if (actor is IMovable mover)
            {
                mover.MoveCCOn();
            }
            actor.AttackOff();
        }
      
    }
}