using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Idle", menuName = "ActorCommand/Player/Idle", order = 1)]
    public class Idle : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            // if(go.IsClimb) return;
            // if(go.IsBlockIdle) return;
            if(!InvokeCondition(go)) return;
            
            if(go.onAir)
                go.SetState(EPlayerState.AirIdle);
            else
                go.SetState(EPlayerState.Idle);
        }

        public override bool InvokeCondition(Player go)
        {
            return go.GetAbleState(EPlayerState.Idle) || go.GetAbleState(EPlayerState.AirIdle);
        }
    }
}