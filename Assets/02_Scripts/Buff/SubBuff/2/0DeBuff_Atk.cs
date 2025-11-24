
namespace Apis
{
    public class Debuff_Atk : Debuff_Stat
    {
        public Debuff_Atk(Buff effect) : base(effect)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_Atk;

        protected override ActorStatType StatType => ActorStatType.Atk;

        public override void OnAdd()
        {
            base.OnAdd();
        }
    }
}