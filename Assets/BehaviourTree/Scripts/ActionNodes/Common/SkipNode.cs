using DG.Tweening;

namespace Apis.BehaviourTreeTool
{
    public class SkipNode : CommonActionNode
    {
        public override void OnStart()
        {
            base.OnStart();
            if(blackBoard.currentNode != null)
            {
                blackBoard.currentNode.OnSkip();
                blackBoard.currentNode = null;
            }
            _actor.transform.DOKill();
            _actor.Rb.DOKill();
        }
        
        public override State OnUpdate()
        {
            return State.Success;
        }
    }
}