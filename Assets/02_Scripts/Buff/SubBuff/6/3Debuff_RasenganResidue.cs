using UnityEngine;

namespace Apis
{
    public class Debuff_RasenganResidue : Debuff_DotDmg
    {

        public Debuff_RasenganResidue(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_RasenganResidue;

        protected override void SetDmg()
        {
            if (GameManager.instance.Player.ActiveSkill is JingburgerActiveSkill skill)
            {
                Dmg = amount[0] * skill.Atk / 100;
            }
            else
            {
                Dmg = 0;
            }
        }
    }
}