using Command;
using UnityEngine;

namespace Command
{
    public abstract class ActorCommand : ScriptableObject, IBufferCommand
    {
        public abstract void Invoke(Actor go);
        public abstract bool InvokeCondition(Actor actor);
    }
}