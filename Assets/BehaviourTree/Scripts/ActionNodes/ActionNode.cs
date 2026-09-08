using DG.Tweening;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public abstract class ActionNode : TreeNode
    {
        /// <summary>
        ///     노드 진입 시 액터를 즉시 정지시킬지 여부. 공격처럼 "행동 전에 멈춰야 하는" 노드가 기본이다.
        ///     연속 이동 노드는 매 진입마다 속도가 지워지면 가속(lerp)이 누적되지 못하므로 false로 둔다.
        ///     <see cref="MovementActionNode" /> 참고.
        /// </summary>
        protected virtual bool StopOnStart => true;

        public override void OnStart()
        {
            _actor.ResetDirection();
            _actor.Rb.DOKill();

            if (StopOnStart && _actor.Rb.bodyType == RigidbodyType2D.Dynamic)
                _actor.Rb.linearVelocity = Vector2.zero;
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