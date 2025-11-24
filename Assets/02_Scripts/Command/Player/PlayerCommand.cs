using UnityEngine;

namespace Command
{
    public abstract class PlayerCommand : ActorCommand
    {
        protected abstract void Invoke(Player go);
        public abstract bool InvokeCondition(Player go);
        public override void Invoke(Actor go)
        {
            if (go is Player player)
            {
                Invoke(player);
            }
            else
            {
                Invoke(GameManager.instance.Player);
            }
        }

        public override bool InvokeCondition(Actor actor)
        {
            if (actor is Player player)
            {
                return InvokeCondition(player);
            }
            else
            {
                return InvokeCondition(GameManager.instance.Player);
            }
        }
    }
}