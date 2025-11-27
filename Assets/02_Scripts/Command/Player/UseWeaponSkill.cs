using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "UseMouseRSkill", menuName = "ActorCommand/Player/UseMouseRSkill")]
    public class UseWeaponSkill : ActorCommand
    {
        public override void Invoke(Actor go)
        {
            // ISkillActive skillType = SkillManager.LeftSkill.ActiveStrategy;
            // if (skillType is InstantSkill or ContinuousSkill or ToggleSkill)
            // {
            //     // PlayerController.PressedKeyDict[KeyCode.Mouse1] = true;
            // }
            //
            // SkillManager.LeftSkill.Use();
        }

        public override bool InvokeCondition(Actor actor)
        {
            //return SkillManager.LeftSkill != null && SkillManager.LeftSkill.TryUse();
            return true;
        }
    }
}