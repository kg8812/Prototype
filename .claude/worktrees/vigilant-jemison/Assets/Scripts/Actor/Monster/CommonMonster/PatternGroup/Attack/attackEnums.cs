using System;
using UnityEngine;

namespace Apis
{
    [Serializable]
    public struct AttackObjectData
    {
        public bool isConsiderDirection;
        public string transformName;
        public Vector3 offset;
        public AttackObject obj;
    }

    public enum TimeType
    {
        Immediately,
        Periodically,
        Randomly
    }

    public enum ProjectileInitialVelocityType
    {
        MonsterDirection,
        ToPlayer
    }
}