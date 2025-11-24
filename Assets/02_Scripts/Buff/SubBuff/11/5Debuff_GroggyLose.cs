using Apis;

namespace Apis
{
    class Debuff_GroggyLose : SubBuff
    {
        public override SubBuffType Type => SubBuffType.Debuff_GroggyLose;

        public Debuff_GroggyLose(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor != null && actor is Monster monster)
            {
                monster.AddGroggyGauge(amount[0]);
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor != null && actor is Monster monster)
            {
                monster.AddGroggyGauge(amount[0]);
            }
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            if (actor != null && actor is Monster monster)
            {
                monster.AddGroggyGauge(amount[0]);
            }
        }
    }
}