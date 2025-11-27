using System;
using System.Collections.Generic;

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