
namespace Apis
{
    public class Buff_CritProb : Buff_Stat
    {
        public Buff_CritProb(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_CritProb;

        protected override ActorStatType StatType => ActorStatType.CritProb;
    }
}