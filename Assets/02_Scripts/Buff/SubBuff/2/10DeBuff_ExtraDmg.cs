
namespace Apis
{
    public class Debuff_ExtraDmg : Debuff_Stat
    {
        public Debuff_ExtraDmg(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_ExtraDmg;

        protected override ActorStatType StatType => ActorStatType.ExtraDmg;
    }
}