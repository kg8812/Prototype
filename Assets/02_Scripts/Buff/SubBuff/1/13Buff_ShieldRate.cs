namespace Apis
{
    public class Buff_ShieldRate : Buff_Stat
    {
        public Buff_ShieldRate(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_ShieldRate;

        protected override ActorStatType StatType => ActorStatType.ShieldRate;
    }
}