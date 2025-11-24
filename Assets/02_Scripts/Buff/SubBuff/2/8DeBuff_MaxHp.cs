
namespace Apis
{
    public class Debuff_MaxHp : Debuff_Stat
    {
        public Debuff_MaxHp(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_MaxHp;

        protected override ActorStatType StatType => ActorStatType.MaxHp;
    }
}