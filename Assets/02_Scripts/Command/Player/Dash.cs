using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Dash", menuName = "ActorCommand/Player/Dash", order = 1)]
    public class Dash : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            go.SetState(EPlayerState.Dash);
        }

        public override bool InvokeCondition(Player player)
        {
            return player.GetAbleState(EPlayerState.Dash)
            && !player.IsDash 
            && player.CoolDown.GetCd(EPlayerCd.Dash) 
            && player.AirDashed < player.MaxAirDash;
        }
    }
}
