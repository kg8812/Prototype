
namespace Apis
{
    public class Debuff_CritProb : Debuff_Stat
    {
        public Debuff_CritProb(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_CritProb;

        protected override ActorStatType StatType => ActorStatType.CritProb;
    }
}