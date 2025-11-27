namespace Apis.BehaviourTreeTool
{
    public class ExcuteAll : CommonCompositeNode
    {
        private int current;

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
                    case State.Failure:
                        current++;
                        break;
                    case State.Running:
                        return State.Running;
                }
            }

            if (current >= children.Count) return State.Success;

            return State.Running;
        }
    }
}