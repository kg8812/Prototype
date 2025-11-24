using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Command
{
    public enum ECommandType {
        None, Special, Attack, Dash, Jump, Heal
    }

    public class InputBuffer
    {
        private readonly Actor user;
        private EActorDirection currentDirection;
        private readonly PriorityQueue<CommandExecutor.CommandInfo, ComparableActionType> actionBuffer;
        private readonly Dictionary<ECommandType, EActorDirection> directionBuffer;
        private readonly Dictionary<ECommandType, Coroutine> timers;

        private ActorController _controller;
        
        public InputBuffer(Actor p,ActorController controller)
        {
            user = p;
            currentDirection = user.Direction;
            if(p is Player player)
                actionBuffer = new(player.MaxActionBufferSize);
            else
                actionBuffer = new(3);
            timers = new();
            directionBuffer = new();
            _controller = controller;
        }

        public void AddEvent(CommandExecutor.CommandInfo input)
        {
            if (!input.IsBufferAction || input.Commands.Count == 0) return;

            var command = input.commandType;

            // TODO: skill도 attack으로 취급했을 때, enqueue 내부에서 중복 체크 제대로 안돼서 둘 다 queue될 수 있음
            if(!actionBuffer.Enqueue(input, new ComparableActionType(command)))
                return;

            if(!directionBuffer.TryAdd(command, currentDirection))
                directionBuffer[command] = currentDirection;

            StartTimer(input);
        }

        public void Routine()
        {
            foreach(var (command,_) in actionBuffer)
            {
                if(!InvokeCondition(command))
                    continue;

                Invoke(command);

                return;
            }
        }

        public bool InvokeCondition(CommandExecutor.CommandInfo input)
        {
            if (input == null || input.Commands.Count == 0) return false;

            bool result = false;

            foreach (var command in input.Commands)
            {
                result = result || command.InvokeCondition(user);
            }
            return result;
        }

        public void Invoke(CommandExecutor.CommandInfo input)
        {
            // string inputName = input.Commands[0].name;
            // if(inputName.Equals("Dash") || inputName.Equals("Attack")) 

            if(timers.TryGetValue(input.commandType, out var prev)){
                GameManager.instance.StopCoroutineWrapper(prev);
                timers.Remove(input.commandType);
            }

            if (directionBuffer.TryGetValue(input.commandType, out var value))
            {
                InvokeDirection(value);
            }

            input.Invoke(user);

            actionBuffer.Remove(input);
            directionBuffer.Remove(input.commandType);
        }

        private void InvokeDirection(EActorDirection direction)
        {
            if (user.Direction == direction || direction == 0) return;

            user.SetDirection(direction);
        }

        public void SetDirection(EActorDirection direction)
        {
            this.currentDirection = direction;
        }
        public EActorDirection GetDirection()
        {
            // 지금 누르고 있는 방향 확인 용
            return currentDirection;
        }

        private void StartTimer(CommandExecutor.CommandInfo command)
        {
            // 기존 타이머 취소
            if(timers.TryGetValue(command.commandType, out var prev)){
                GameManager.instance.StopCoroutineWrapper(prev);
                timers.Remove(command.commandType);
            }

            // 새 타이머 시작
            IEnumerator timeout(){
                yield return new WaitForSeconds(command.timeout);
                // timeout시 timer list에서 coroutine 제거, 버퍼 돼 있는 커맨드 제거 
                timers.Remove(command.commandType);
                actionBuffer.Remove(command);
                directionBuffer.Remove(command.commandType);
            }

            timers.Add(command.commandType, GameManager.instance.StartCoroutineWrapper(timeout()));
        }
        
        public void ClearBuffer()
        {
            actionBuffer.Clear();
            directionBuffer.Clear();
        }
    }

    public interface IBufferCommand
    {
        public bool InvokeCondition(Actor actor);      // 커맨드 실행 조건
    }

    public class ComparableActionType : IComparable<ComparableActionType>
    {
        private readonly ECommandType _commandType;
        public ECommandType CommandType => _commandType;   
        public ComparableActionType(ECommandType commandType)
        {
            _commandType = commandType;
        }

        public int CompareTo(ComparableActionType other)
        {
            if(Equals(other)) return 0;

            GameManager GI = GameManager.instance;

            if(GI == null || GI.Player == null) return 0;

            var current = GI.Player.CurrentCommand;

            var rank = GetCommandRank(current);

            return rank[(int)CommandType] - rank[(int)other.CommandType];
        }

        public static int[] GetCommandRank(ECommandType currentCommand)
        {
            return currentCommand switch
            {
                ECommandType.Attack => new int[] { 6, 1, 4, 2, 3, 5 },
                ECommandType.Dash => new int[] { 6, 1, 3, 4, 2, 5 },
                _ => new int[] { 6, 1, 2, 3, 4, 5 },
            };
        }
    } 
}
