using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Jump", menuName = "ActorCommand/Player/Jump", order = 1)]
    public class Jump : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            if (!InvokeCondition(go)) return;

            go.SetState(EPlayerState.Jump);
        }

        public override bool InvokeCondition(Player player)
        {
            return player.GetAbleState(EPlayerState.Jump)
                   && player.CoyoteCurrentJump < player.playerStat.JumpMax
                   && player.CoolDown.GetCd(EPlayerCd.DashToJump);
        }
    }
}