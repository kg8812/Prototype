using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class TestNodeScript : BossActionNode
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
            return State.Success;
        }
    }
}