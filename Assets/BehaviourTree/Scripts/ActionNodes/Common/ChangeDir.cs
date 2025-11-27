namespace Apis.BehaviourTreeTool
{
    public class ChangeDir : CommonActionNode
    {
        public override State OnUpdate()
        {
            _actor.Flip();

            return State.Success;
        }
    }
}