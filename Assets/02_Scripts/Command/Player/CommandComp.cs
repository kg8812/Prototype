using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Command
{
    [CreateAssetMenu(fileName = "CommandComp", menuName = "ActorCommand/CommandComp", order = 0)]
    public class CommandComp : SerializedScriptableObject
    {
        public Dictionary<Define.GameKey, CommandList> Commands;
        public ActorCommand inputEmptyMove;
        public ActorCommand inputIdle;
    }
}