using DG.Tweening;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public abstract class ActionNode : TreeNode
    {
        public override void OnStart()
        {
            _actor.ResetDirection();
            _actor.Rb.DOKill();

            if (_actor.Rb.bodyType == RigidbodyType2D.Dynamic) _actor.Rb.linearVelocity = Vector2.zero;
        }

        public override State Update()
        {
            base.Update();
            if (state == State.Running || state == State.Success)
            {
                blackBoard.currentNodeName = description;
                blackBoard.currentNode = this;
            }

            return state;
        }

        public override void OnStop()
        {
            _actor.ResetDirection();
        }
    }
}