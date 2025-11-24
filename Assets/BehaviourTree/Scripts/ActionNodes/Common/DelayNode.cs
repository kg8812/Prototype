using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class DelayNode : CommonActionNode
    {
        public float duration;
        float _startTime;

        public override void OnStart()
        {            
            base.OnStart();
            _startTime = Time.time;
        }

        public override State OnUpdate()
        {
            if((Time.time - _startTime) > duration && duration > 0)
            {
                return State.Success;
            }
            return State.Running;
        }
    }
}