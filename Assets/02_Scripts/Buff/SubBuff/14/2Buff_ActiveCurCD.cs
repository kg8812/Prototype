
namespace Apis
{
    public class Buff_ActiveCurCD : SubBuff
    {
        public Buff_ActiveCurCD(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if(actor is Player player && player.ActiveSkill != null)
            {
                player.ActiveSkill.CurCd += amount[0];
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if(actor is Player player && player.ActiveSkill != null)
            {
                player.ActiveSkill.CurCd -= amount[0];
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            if(actor is Player player && player.ActiveSkill != null)
            {
                player.ActiveSkill.CurCd += amount[0];
            }
        }

        public override SubBuffType Type => SubBuffType.Buff_ActiveCurCD;
    }
}