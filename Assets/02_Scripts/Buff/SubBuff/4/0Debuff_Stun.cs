using Apis;

namespace Apis
{
    public class Debuff_Stun : Debuff_CC
    {
        public Debuff_Stun(Buff effect) : base(effect)
        {
            isStunImmune = !actor.IsAffectedByCC;
            monster = actor as Monster;
        }

        public override SubBuffType Type => SubBuffType.Debuff_Stun;

        private readonly bool isStunImmune;
        private readonly Monster monster;

        public override void OnAdd()
        {
            base.OnAdd();

            if(isStunImmune)
            {
                monster?.AddGroggyGauge(amount[0]);
            }
        }

        protected override void OnTypeAdd()
        {
            base.OnTypeAdd();
        }

        protected override void OnTypeRemove()
        {
            base.OnTypeRemove();
            actor.EndStun();
            RemoveEffect();
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            if(isStunImmune)
            {
                monster.AddGroggyGauge(amount[0]);
            }
        }
    }
}
