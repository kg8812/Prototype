namespace Apis
{
    public class Buff_DmgReduce : Buff_Stat
    {
        public Buff_DmgReduce(Buff effect) : base(effect)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
        }

        public override SubBuffType Type => SubBuffType.Buff_DmgReduce;

        protected override ActorStatType StatType => ActorStatType.DmgReduce;
    }
}
