using Apis;
using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "EndWeaponSkill", menuName = "ActorCommand/Player/EndWeaponSkill")]
    public class EndWeaponSkill : ActorCommand
    {
        public int index;

        public override void Invoke(Actor go)
        {
            var item = AttackItemManager.AtkInven.AtkItemInven[index] as IAttackItem;
            item?.EndAttack();
        }

        public override bool InvokeCondition(Actor actor)
        {
            return true;
        }
    }
}