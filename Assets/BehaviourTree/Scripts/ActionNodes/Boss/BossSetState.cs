
namespace Apis.BehaviourTreeTool
{
    public class BossSetState : BossActionNode
    {
        public BossMonster.BossState bossState;

        public override void OnStart()
        {
            base.OnStart();
            boss.SetState(bossState);
        }
        
        public override State OnUpdate()
        {
            return State.Success;
        }
    }
}