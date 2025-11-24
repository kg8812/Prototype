namespace Apis
{
    public class Buff_HealRate : Buff_Stat
    {
        public Buff_HealRate(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override SubBuffType Type => SubBuffType.Buff_HealRate;

        protected override ActorStatType StatType => ActorStatType.HealRate;
    }
}