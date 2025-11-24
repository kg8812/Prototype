using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Heal", menuName = "ActorCommand/Player/Heal", order = 1)]
    public class Heal : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            go.SetState(EPlayerState.Heal);
        }

        public override bool InvokeCondition(Player player)
        {
            return player.GetAbleState(EPlayerState.Heal) && !(player.CurrentPotionCapacity < 1);
        }
    }
}
