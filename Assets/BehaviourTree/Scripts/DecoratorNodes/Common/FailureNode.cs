namespace Apis.BehaviourTreeTool
{
    public class FailureNode : CommonDecoratorNode
    {
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            return State.Failure;
        }

        public override bool Check()
        {
            return false;
        }
    }
}