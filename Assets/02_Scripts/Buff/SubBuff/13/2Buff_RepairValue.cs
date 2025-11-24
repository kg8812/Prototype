namespace Apis
{
    public class Buff_RepairValue : SubBuff
    {
        public Buff_RepairValue(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor is Player player)
            {
                player.repairRatio += amount[0];
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is Player player)
            {
                player.repairRatio -= amount[0];
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor is Player player)
            {
                player.repairRatio += amount[0];
            }
        }

        public override SubBuffType Type => SubBuffType.Buff_RepairValue;
    }
}