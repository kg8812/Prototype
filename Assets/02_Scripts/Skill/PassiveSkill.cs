using UnityEngine;

namespace Apis
{
    public abstract class PassiveSkill : Skill
    {
        public override bool TryUse()
        {
            return CDActive.CheckActive();
        }

        public override bool Use()
        {
            if (!base.Use()) return false;
            
            Active();

            return true;
        }
    }
}