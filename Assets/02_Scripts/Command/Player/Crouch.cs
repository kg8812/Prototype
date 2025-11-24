using UnityEngine;
namespace Command
{
    [CreateAssetMenu(fileName = "Crouch", menuName = "ActorCommand/Player/Crouch", order = 1)]    
    public class Crouch : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            if(!InvokeCondition(go)) return;
            go.SetState(EPlayerState.Crouch);
        }

        public override bool InvokeCondition(Player go)
        {
            return go.GetAbleState(EPlayerState.Crouch);
        }
    }
}
