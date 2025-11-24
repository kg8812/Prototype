
namespace Apis
{
    public class Buff_ActiveDuration : SubBuff
    {
        public Buff_ActiveDuration(Buff buff) : base(buff)
        {
            attachment = new SkillAttachment(new SkillStat(0, 0, amount[0], 0, 0));
        }

        private ISkill attachment;
        
        public override void OnAdd()
        {
            base.OnAdd();
            if(actor is Player player)
            {
                player.ActiveSkill.AddAttachment(attachment);
            }
        }
        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is Player player)
            {
                player.ActiveSkill.RemoveAttachment(attachment);
            }
        }
        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor is Player player)
            {
                player.ActiveSkill.AddAttachment(attachment);
            }
        }
        
        public override SubBuffType Type => SubBuffType.Buff_ActiveDuration;
    }
}