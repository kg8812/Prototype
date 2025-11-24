using UnityEngine;

namespace Command{
    [CreateAssetMenu(fileName = "IceDrillCharge", menuName = "ActorCommand/Player/IceDrillCharge")]
    public class IceDrillCharge : PlayerCommand
    {
        [SerializeField] private EActorDirection direction;
        protected override void Invoke(Player go)
        {
            if(!InvokeCondition(go)) return;

            go.SetDirection(direction);

            go.SetState(EPlayerState.IceDrillCharge);
        }

        public override bool InvokeCondition(Player go)
        {
            return go.GetAbleState(EPlayerState.IceDrillCharge) && go.Controller.IsPressing(Define.GameKey.Dash);
        }
    }
}