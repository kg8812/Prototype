using Command;
using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "UseActiveSkill", menuName = "ActorCommand/Common/ActiveSkill")]

    public class UseActiveSkill : ActorCommand
    {
        public override void Invoke(Actor go)
        {
            if (GameManager.instance.Player.ActiveSkill == null)
            {
                Debug.Log("액티브 스킬이 없음");
                return;
            }
            GameManager.instance.Player.ActiveSkill.Use();
        }

        public override bool InvokeCondition(Actor actor)
        {
            return GameManager.instance.Player?.ActiveSkill != null && GameManager.instance.Player.ActiveSkill.TryUse();
        }
    }
}