using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "ChainAction",menuName = "ActorCommand/Player/ChainAction")]
    public class UseChainAction : PlayerCommand
    { 
        protected override void Invoke(Player go)
        {
            go.ActionController.DoChainAction();
        }

        public override bool InvokeCondition(Player go)
        {
            return go.ActionController.enteredChainObjects.Count > 0;
        }
    }
}