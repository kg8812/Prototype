
namespace Apis
{
    public class Buff_RepairCount : SubBuff
    {
        public Buff_RepairCount(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if(actor is Player player)
            {
                player.IncreaseMaxPotion((int)amount[0]);
            }
        }
        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is Player player)
            {
                player.IncreaseMaxPotion(-(int)amount[0]);
            }
        }
        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor is Player player)
            {
                player.IncreaseMaxPotion((int)amount[0]);
            }
        }
        
        public override SubBuffType Type => SubBuffType.Buff_RepairCount;
    }
}