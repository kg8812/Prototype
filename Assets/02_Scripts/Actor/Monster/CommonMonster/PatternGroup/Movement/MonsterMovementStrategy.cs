using UnityEngine;

namespace Apis
{
    public abstract class MonsterMovementStrategy: ScriptableObject
    {
        public abstract void Movement(Monster monster);
    }
}