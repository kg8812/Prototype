using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "UseIceDrill",menuName = "ActorCommand/Player/IceDrill")]
    public class UseIceDrill : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            go.AnimController.Trigger(EAnimationTrigger.IceDrillOn);
        }

        public override bool InvokeCondition(Player go)
        {
            return go != null && go.CurrentState == EPlayerState.IceDrillCharge;
        }
    }
}