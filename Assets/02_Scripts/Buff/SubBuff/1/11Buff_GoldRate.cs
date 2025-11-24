namespace Apis
{
    public class Buff_GoldRate : Buff_Stat
    {
        public Buff_GoldRate(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_GoldRate;

        protected override ActorStatType StatType => ActorStatType.GoldRate;
    }
}