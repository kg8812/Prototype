
namespace Apis
{
    public class Buff_ActiveCD : SubBuff
    {
        public Buff_ActiveCD(Buff buff) : base(buff)
        {

            attachment = new SkillAttachment(new SkillStat(amount[0], 0, 0, 0, 0));
        }

        private SkillAttachment attachment;
        
        public override void OnAdd()
        {
            base.OnAdd();
            if(actor is Player player && player.ActiveSkill != null)
            {
                player.ActiveSkill.AddAttachment(attachment);
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is Player player && player.ActiveSkill != null)
            {
                player.ActiveSkill.RemoveAttachment(attachment);
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            if (actor is Player player && player.ActiveSkill != null)
            {
                player.ActiveSkill.AddAttachment(attachment);
            }
        }
        
        public override SubBuffType Type => SubBuffType.Buff_ActiveCD;
    }
}