using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class MoveNode : CommonActionNode
    {
        public float time;
        float startTime;
        private IMovable mover;
        public override void OnStart()
        {
            base.OnStart();
            startTime = Time.time;
            mover = _actor as IMovable;
        }
        
        public override State OnUpdate()
        {
            if (Mathf.Approximately(0 ,time))
            {
                mover?.ActorMovement.Move(_actor.Direction, 1);

                return State.Success;
            }
            
            if (startTime + time > Time.time)
            {
                return State.Success;
            }

            mover?.ActorMovement.Move(_actor.Direction, 1);

            return State.Running;
        }
    }
}