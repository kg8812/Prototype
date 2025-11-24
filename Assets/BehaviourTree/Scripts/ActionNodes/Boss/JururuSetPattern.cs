namespace Apis.BehaviourTreeTool
{
    public class JururuSetPattern : BossActionNode
    {
        public BlackBoard.JururuBoss.Patterns pattern;
        
    
        public override State OnUpdate()
        {
            blackBoard.jururuBoss.pattern = pattern;
            return State.Success;
        }
    }
}