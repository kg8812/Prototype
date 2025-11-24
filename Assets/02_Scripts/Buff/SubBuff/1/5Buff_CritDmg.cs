
namespace Apis
{
    public class Buff_CritDmg : Buff_Stat
    {
        public Buff_CritDmg(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_CritDmg;

        protected override ActorStatType StatType => ActorStatType.CritDmg;
    }
}
