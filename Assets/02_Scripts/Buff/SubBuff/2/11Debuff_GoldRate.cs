
namespace Apis
{
    public class Debuff_GoldRate : Debuff_Stat
    {
        public override SubBuffType Type => SubBuffType.Debuff_GoldRate;

        public Debuff_GoldRate(Buff buff) : base(buff)
        {
        }

        protected override ActorStatType StatType => ActorStatType.GoldRate;
    }
}