namespace Apis
{
    public class Debuff_ShieldRate : Debuff_Stat
    {
        public Debuff_ShieldRate(Buff buff) : base(buff)
        {
        }


        public override SubBuffType Type => SubBuffType.Debuff_ShieldRate;

        protected override ActorStatType StatType => ActorStatType.ShieldRate;
    }
}