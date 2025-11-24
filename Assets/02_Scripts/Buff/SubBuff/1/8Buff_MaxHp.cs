
namespace Apis
{
    public class Buff_MaxHp : Buff_Stat
    {
        public Buff_MaxHp(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_MaxHp;

        protected override ActorStatType StatType => ActorStatType.MaxHp;
    }
}