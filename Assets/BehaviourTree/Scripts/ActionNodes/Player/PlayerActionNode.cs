
namespace Apis.BehaviourTreeTool
{
    public abstract class PlayerActionNode : ActionNode
    {
        protected Player player { get; private set; }
        public override void SetActor(Actor actor)
        {
            base.SetActor(actor);

            player = actor as Player;
        }
    }
}