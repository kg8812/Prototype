
namespace Apis
{
    public class Debuff_Mentality : Debuff_Stat
    {
        public Debuff_Mentality(Buff effect) : base(effect)
        {

        }

        public override SubBuffType Type => SubBuffType.Debuff_Mentality;

        protected override ActorStatType StatType => ActorStatType.Mental;
    }
}