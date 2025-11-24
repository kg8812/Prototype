using UnityEngine;

namespace Command{
    [CreateAssetMenu(fileName = "Run", menuName = "ActorCommand/Player/Run")]
    public class Run : PlayerCommand
    {
        [SerializeField] private EActorDirection direction;
        protected override void Invoke(Player go)
        {
            if(!InvokeCondition(go)) return;

            go.SetDirection(direction);

            if(!go.onAir)
                go.SetState(EPlayerState.Run);
            else
                go.SetState(EPlayerState.AirRun);
        }

        public override bool InvokeCondition(Player go)
        {
            return go.GetAbleState(EPlayerState.Run) && go.Controller.IsPressing(Define.GameKey.Dash);
        }
    }
}
