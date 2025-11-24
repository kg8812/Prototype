using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    [Serializable]
    public class CommandList 
    {
        public List<ActorCommand> keyDownCommand = new();
        public List<ActorCommand> keyUpCommand = new();
        public List<ActorCommand> keyCommand = new();
    }
}