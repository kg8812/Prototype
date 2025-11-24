using System;
using UnityEngine;
namespace Command
{
    [CreateAssetMenu(fileName = "Move", menuName = "ActorCommand/Player/Move", order = 1)]
    public class Move : PlayerCommand
    {
        [SerializeField] private EActorDirection direction; // 좌우 확인

        protected override void Invoke(Player go)
        {
            if(direction == EActorDirection.Right && go.PressingLR[0]) return;

            if(!InvokeCondition(go)) return;
            
            go.SetDirection(direction);

            if(go.CurrentState == EPlayerState.Charging) return;

            if(go.onAir)
                go.SetState(EPlayerState.AirMove);
            else
                go.SetState(EPlayerState.Move);
        }

        public override bool InvokeCondition(Player go)
        {
            // run 가능한 상태에서는 발동 X
            if((go.GetAbleState(EPlayerState.Run)||go.GetAbleState(EPlayerState.IceDrillCharge)) && go.Controller.IsPressing(Define.GameKey.Dash)) return false;

            return go.GetAbleState(EPlayerState.Move) || go.GetAbleState(EPlayerState.AirMove) || (go.CurrentState == EPlayerState.Charging);
        }
    }
}