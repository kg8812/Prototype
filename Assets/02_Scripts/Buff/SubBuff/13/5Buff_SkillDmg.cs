namespace Apis
{
    public class Buff_SkillDmg : SubBuff
    {
        public Buff_SkillDmg(Buff buff) : base(buff)
        {
        }

        public override void OnAdd()
        {
            base.OnAdd();
            if (actor is Player player)
            {
            }
        }

        public override void OnRemove()
        {
            base.OnRemove();
            if (actor is Player player)
            {
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();

            if (actor is Player player)
            {
            }
        }

        public override SubBuffType Type => SubBuffType.Buff_SkillDmg;
    }
}