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
            
            go.SetState(EPlayerState.Move);
        }

        public override bool InvokeCondition(Player go)
        {
            return true;
        }
    }
}