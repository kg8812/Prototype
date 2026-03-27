namespace Apis.BehaviourTreeTool
{
    public class RepeatNode : CommonDecoratorNode
    {
        public int count;

        private int curCount;

        public override bool Check()
        {
            return CheckChild;
        }

        public override void OnStart()
        {
            curCount = 0;
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (count == 0)
            {
                child.Update();
                return State.Running;
            }

            switch (child.Update())
            {
                case State.Success:
                case State.Failure:
                    curCount++;
                    return curCount >= count ? State.Success : State.Running;
                case State.Running:
                    return State.Running;
            }

            return State.Running;
        }
    }
}