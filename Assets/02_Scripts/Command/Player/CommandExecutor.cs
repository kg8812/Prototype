using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Command
{
    [CreateAssetMenu(fileName = "Executor", menuName = "ActorCommand/Executor", order = 0)]
    public class CommandExecutor : ScriptableObject
    {
        public CommandInfo keyDownCommand = new();
        public CommandInfo keyUpCommand = new();
        public CommandInfo keyCommand = new();

        private ActorController _controller;

        public void Init(ActorController controller)
        {
            _controller = controller;
        }

        public void SetDirectionToLeft()
        {
            _controller.BufferSetDirection(EActorDirection.Left);
        }

        public void SetDirectionToRight()
        {
            _controller.BufferSetDirection(EActorDirection.Right);
        }

        [Serializable]
        public class CommandInfo
        {
            public bool IsBufferAction;
            [HideInInspector] public List<ActorCommand> Commands = new();
            public UnityEvent pressedAction;
            public ECommandType commandType; // 버퍼 우선순위 처리 용
            public float timeout; // 버퍼 타임아웃 시간

            public void Invoke(Actor actor)
            {
                Commands?.ForEach(x =>
                {
                    if (x == null) return;

                    if (x.InvokeCondition(actor)) x.Invoke(actor);
                });
            }

            public override bool Equals(object obj)
            {
                if (obj is CommandInfo other) return commandType == other.commandType;
                return false;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}