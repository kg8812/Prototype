
namespace Apis
{
    public class Debuff_MoveSpeed : Debuff_Stat
    {
        public Debuff_MoveSpeed(Buff effect) : base(effect)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override SubBuffType Type => SubBuffType.Debuff_MoveSpeed;

        protected override ActorStatType StatType => ActorStatType.MoveSpeed;
    }
}