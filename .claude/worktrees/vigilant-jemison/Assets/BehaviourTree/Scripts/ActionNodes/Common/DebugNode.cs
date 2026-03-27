using UnityEngine;
using UnityEngine.Serialization;

namespace Apis.BehaviourTreeTool
{
    public class DebugNode : CommonActionNode
    {
        [FormerlySerializedAs("Message")] public string message;

        public override void OnStart()
        {
            base.OnStart();
            Debug.Log("Start " + message);
        }

        public override void OnStop()
        {
            base.OnStop();

            Debug.Log("Stop" + message);
        }

        public override State OnUpdate()
        {
            Debug.Log(message);

            return State.Success;
        }
    }
}