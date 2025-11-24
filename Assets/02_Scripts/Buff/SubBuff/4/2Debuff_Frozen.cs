using System;
using DG.Tweening;

namespace Apis
{
    public class Debuff_Frozen : Debuff_CC
    {
        public Debuff_Frozen(Buff effect) : base(effect)
        {
           
        }

        public override SubBuffType Type => SubBuffType.Debuff_Frozen;
        
        public override void OnAdd()
        {
            base.OnAdd();
           
        }
        public override void OnRemove()
        {
            base.OnRemove();
        }

        protected override void OnTypeAdd()
        {
            base.OnTypeAdd();
            actor.TurnFrozenOn();
            if (actor is IMovable mover)
            {
                mover.MoveCCOn();
            }
            actor.AnimPauseOn();

            Sequence seq = DOTween.Sequence();
            Guid guid = actor.AddSubBuffTypeImmune(SubBuffType.Debuff_Chill);
            seq.SetDelay(3);
            seq.AppendCallback(() => actor.RemoveSubBuffTypeImmune(SubBuffType.Debuff_Chill,guid));
        }

        protected override void OnTypeRemove()
        {
            base.OnTypeRemove();
            actor.TurnFrozenOff();
            actor.AnimPauseOff();
            if (actor is IMovable mover)
            {
                mover.MoveCCOff();
            }
        }
    }
}
