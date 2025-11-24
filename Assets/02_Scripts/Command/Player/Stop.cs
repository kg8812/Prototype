using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Stop", menuName = "ActorCommand/Player/Stop", order = 1)]
    public class Stop : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            // go.resister.Resist();

            if(!InvokeCondition(go)) return;

            go.SetState(EPlayerState.Stop);
        }

        public override bool InvokeCondition(Player go)
        {
            // Debug.Log(go.IsDash + " " + go.OnAttack);
            return go.GetAbleState(EPlayerState.Stop);
        }
    }
}