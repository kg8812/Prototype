namespace Apis.BehaviourTreeTool
{
    public class CheckPlayerDead : CommonDecoratorNode
    {
        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (GameManager.instance.Player == null || GameManager.instance.Player.IsDead) return State.Success;

            return State.Failure;
        }

        public override bool Check()
        {
            return GameManager.instance.Player == null || GameManager.instance.Player.IsDead;
        }
    }
}