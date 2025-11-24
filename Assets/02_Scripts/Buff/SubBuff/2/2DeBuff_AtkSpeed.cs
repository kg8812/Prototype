
namespace Apis
{
    public class Debuff_AtkSpeed : Debuff_Stat
    {
        public Debuff_AtkSpeed(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_AtkSpeed;

        protected override ActorStatType StatType => ActorStatType.AtkSpeed;
    }
}