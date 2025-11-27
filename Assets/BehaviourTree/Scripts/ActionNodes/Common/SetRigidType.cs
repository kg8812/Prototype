using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class SetRigidType : CommonActionNode
    {
        public RigidbodyType2D bodyType;
        private IMovable mover;

        public override void OnStart()
        {
            base.OnStart();
            mover = _actor as IMovable;
        }

        public override State OnUpdate()
        {
            _actor.Rb.bodyType = bodyType;
            if (bodyType == RigidbodyType2D.Static)
                mover.ActorMovement.SetGravityToZero();
            else
                mover.ActorMovement.ResetGravity();
            return State.Success;
        }
    }
}