using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "Drop", menuName = "ActorCommand/Player/Drop", order = 1)]
    public class Drop : PlayerCommand
    {
        protected override void Invoke(Player go)
        {
            if(!InvokeCondition(go)) return;

            go.SetState(EPlayerState.Drop);
        }

        public override bool InvokeCondition(Player go)
        {
            return go.GetAbleState(EPlayerState.Drop) && go.IsDropable(out var _);
        }
    }
}
