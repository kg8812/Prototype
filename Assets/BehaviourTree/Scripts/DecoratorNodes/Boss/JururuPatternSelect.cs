
namespace Apis.BehaviourTreeTool
{
    public class JururuPatternSelect : BossDecoratorNode
    {
        public BlackBoard.JururuBoss.Patterns pattern;
        public override void OnStart()
        {
        }
    
        public override void OnStop()
        {

        }
    
        public override State OnUpdate()
        {
            if (blackBoard.jururuBoss.pattern == pattern)
            {
                return child.Update();
            }

            return State.Failure;
        }
        public override bool Check()
        {
            if (blackBoard.jururuBoss.pattern == pattern)
            {
                return CheckChild;
            }

            return false;
        }
    }
}