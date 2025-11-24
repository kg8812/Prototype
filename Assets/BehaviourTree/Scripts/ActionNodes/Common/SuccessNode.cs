namespace Apis.BehaviourTreeTool
{
    public class SuccessNode : CommonActionNode
    {
        public override State OnUpdate()
        {
            return State.Success;
        }
    }
}