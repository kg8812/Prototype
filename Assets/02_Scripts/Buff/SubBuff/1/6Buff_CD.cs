
namespace Apis
{
    public class Buff_CD : Buff_Stat
    {
        public Buff_CD(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_CD;

        protected override ActorStatType StatType => ActorStatType.CDReduction;
    }
}