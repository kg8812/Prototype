
namespace Apis
{
    public class Buff_AtkSpeed :Buff_Stat
    {
        public Buff_AtkSpeed(Buff effect) : base(effect)
        {
            
        }

        public override SubBuffType Type => SubBuffType.Buff_AtkSpeed;

        protected override ActorStatType StatType => ActorStatType.AtkSpeed;
    }
}
