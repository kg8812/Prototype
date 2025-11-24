
namespace Apis
{
    public class Debuff_CD : Debuff_Stat
    {
        public Debuff_CD(Buff effect) : base(effect)
        {

        }

        public override SubBuffType Type => SubBuffType.Debuff_CD;

        protected override ActorStatType StatType => ActorStatType.CDReduction;
    }
}