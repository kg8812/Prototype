using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class WaitForAlert : CommonActionNode
    {
        public string message;

        bool isAlert;

        public float maxTime;

        float startTime;
        public override void OnStart()
        {
            base.OnStart();
            startTime = Time.time;
            OnAlert.RemoveAllListeners();
            isAlert = false;
            OnAlert.AddListener(Invoke);
        }
    
        public override void OnStop()
        {
            base.OnStop();
            OnAlert.RemoveListener(Invoke);
        }
    
        public override State OnUpdate()
        {
            if(maxTime > 0 && startTime + maxTime < Time.time)
            {
                return State.Success;
            }
            
            return isAlert ? State.Success : State.Running;
        }

        void Invoke(string message)
        {
            if(message == this.message)
            {
                isAlert = true;
            }
        }
    }
}