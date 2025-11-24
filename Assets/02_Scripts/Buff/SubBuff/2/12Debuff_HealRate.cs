namespace Apis
{
    public class Debuff_HealRate : Debuff_Stat
    {
        public Debuff_HealRate(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_HealRate;

        protected override ActorStatType StatType => ActorStatType.HealRate;
    }
}