namespace Apis.BehaviourTreeTool
{
    public class BossChangeDir : BossActionNode
    {
        public override State OnUpdate()
        {
            if (GameManager.instance.Player == null) return State.Failure;

            var dirX = GameManager.instance.ControllingEntity.Position.x - boss.Position.x;
            boss.SetDirection(dirX < 0 ? EActorDirection.Left : EActorDirection.Right);

            return State.Success;
        }
    }
}