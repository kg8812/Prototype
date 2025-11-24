
namespace Apis.BehaviourTreeTool
{
    public class SummonsCheck : BossActionNode
    {
       
        public override State OnUpdate()
        {
            return blackBoard.jururuBoss.summons.Count == 0 ? State.Success : State.Running;
        }
    }
}