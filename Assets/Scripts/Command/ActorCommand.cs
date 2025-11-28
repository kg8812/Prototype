using UnityEngine;

namespace Command
{
    public abstract class ActorCommand : ScriptableObject, IBufferCommand
    {
        public abstract bool InvokeCondition(Actor actor);
        public abstract void Invoke(Actor go);
    }
}