using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class SetDelayTime : CommonDecoratorNode
    {
        public float minTime;
        public float maxTime;

        public override void OnStart()
        {
            var rand = Random.Range(minTime, maxTime);

            if (child is DelayNode delayNode) delayNode.duration = rand;
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            return child.Update();
        }

        public override bool Check()
        {
            return CheckChild;
        }
    }
}