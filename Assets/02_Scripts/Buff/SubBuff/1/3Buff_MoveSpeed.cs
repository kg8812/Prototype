
namespace Apis
{
    public class Buff_MoveSpeed : Buff_Stat
    {
        public Buff_MoveSpeed(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_MoveSpeed;

        protected override ActorStatType StatType => ActorStatType.MoveSpeed;
    }
}