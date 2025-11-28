using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Interact", menuName = "ActorCommand/Common/Interact")]
    public class Interact : PlayerCommand
    {
        public override void Invoke(Actor go)
        {
            Player player = null;
            if (go is Player go1) player = go1;

            if (go is Summon { Master: Player go2 }) player = go2;

            if (player == null || !player.isInteractable) return;

            var interact = player.getInteract();
            if (interact != null && interact.IsInteractable())
            {
                player.SetState(EPlayerState.Idle);
                // player.SetState(EPlayerState.Interact);
                interact.OnInteract();
                // player.animator.SetTrigger("Interact");
            }
        }

        protected override void Invoke(Player go)
        {
        }

        public override bool InvokeCondition(Player player)
        {
            if (player == null) return true;

            var currentState = player.GetState();
            return currentState is EPlayerState.Idle or EPlayerState.Move;
        }
    }
}