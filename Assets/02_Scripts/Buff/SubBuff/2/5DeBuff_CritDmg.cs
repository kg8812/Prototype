
namespace Apis
{
    public class Debuff_CritDmg : Debuff_Stat
    {
        public Debuff_CritDmg(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_CritDmg;

        protected override ActorStatType StatType => ActorStatType.CritDmg;
    }
}