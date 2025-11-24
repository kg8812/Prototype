
namespace Apis.BehaviourTreeTool
{
    public class SequenceNode : CommonCompositeNode
    {
        int current;
        
        public override void OnStart()
        {

            base.OnStart();
            current = 0;
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            while (current < children.Count)
            {
                var child = children[current];

                
                switch (child.Update())
                {
                    case State.Success:
                        current++;
                        break;
                    case State.Failure:
                        return State.Failure;
                    case State.Running:
                        return State.Running;
                }
            }
            if(current >= children.Count)
            {
                return State.Success;
            }
            else
            {
                return State.Running;
            }
        }
    }
}