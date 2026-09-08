using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class CommonAttackNode : CommonActionNode
    {
        public override void OnStart()
        {     
            base.OnStart();     
        }
    
        public override void OnStop()
        {
        }
    
        public override State OnUpdate()
        {
            _actor.AttackOn();
            return State.Success;
        }
    }
}