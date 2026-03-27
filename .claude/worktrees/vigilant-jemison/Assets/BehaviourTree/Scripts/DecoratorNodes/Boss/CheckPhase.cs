namespace Apis.BehaviourTreeTool
{
    public class CheckPhase : BossDecoratorNode
    {
        public BossMonster.BossPhase phase;

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (boss.phase == phase) return child.Update();

            return State.Failure;
        }

        public override bool Check()
        {
            if (boss.phase == phase) return CheckChild;
            return false;
        }
    }
}