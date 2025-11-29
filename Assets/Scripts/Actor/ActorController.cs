using System.Collections.Generic;
using System.Linq;
using Command;
using Save.Schema;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActorController : SerializedMonoBehaviour, IController
{
    public Dictionary<Define.GameKey, CommandExecutor> Executors;
    [HideInInspector] public ActorCommand inputEmptyMove;
    [HideInInspector] public ActorCommand inputIdle;
    private CoyoteVar<bool> _IsPressingDown;

    [SerializeField] private CommandComp baseCommands;

    [LabelText("버퍼 지속시간")] public float bufferDuration;

    protected Actor user;

    private InputBuffer buffer;

    private Dictionary<Define.GameKey, bool> pressedKeys;
    private bool isEnabled;
    private bool valid => baseCommands == null;
    public bool IsPressingDown => _IsPressingDown;
    public ECommandType CurrentCommand { get; private set; }

    public CommandComp BaseCommands => baseCommands;
    public EActorDirection PressingDir => buffer.GetDirection();

    protected virtual void Awake()
    {
        ReturnToBase();
        user = GetComponent<Actor>();
        buffer = new InputBuffer(user, this);
        Executors = Executors.ToDictionary(x => x.Key, x => Instantiate(x.Value));
        Executors.ForEach(x => x.Value.Init(this));
        _IsPressingDown = new CoyoteVar<bool>(0.3f);
        pressedKeys = new Dictionary<Define.GameKey, bool>();
        isEnabled = true;
    }

    public virtual void KeyControl()
    {
        if (!isEnabled) return;
        
        foreach (var x in Executors)
        {
            if (InputManager.GetKeyDown(GetKeyCode(x.Key)))
            {
                Press(x.Value.keyDownCommand);
                KeyDown(x.Key);
            }

            if (InputManager.GetKey(GetKeyCode(x.Key)))
            {
                Debug.Log(x.Value);
                Press(x.Value.keyCommand);
            }
            else
            {
                if (KeyUp(x.Key)) Press(x.Value.keyUpCommand);
            }
        }

        if (!(Input.GetKey(DataAccess.Settings.Data.gameKeys[Define.GameKey.LeftMove]) ||
              Input.GetKey(DataAccess.Settings.Data.gameKeys[Define.GameKey.RightMove])))
        {
            inputEmptyMove?.Invoke(user);
            BufferSetDirection(0);
        }

        if (!InputManager.AnyKey())
        {
            inputIdle?.Invoke(user);
            BufferSetDirection(0);
        }

        buffer?.Routine();
    }

    public virtual void GamePadControl()
    {
        foreach (var x in Executors)
        {
            if (x.Key is Define.GameKey.LeftMove or Define.GameKey.RightMove or Define.GameKey.Down
                or Define.GameKey.Up) continue;

            var button = KeySettingManager.GetGameButton(x.Key);
            if (button == null) continue;

            if (InputManager.GetButtonDown(button))
            {
                Press(x.Value.keyDownCommand);
                KeyDown(x.Key);
            }

            if (InputManager.GetButton(button))
            {
                Press(x.Value.keyCommand);
            }
            else
            {
                if (KeyUp(x.Key)) Press(x.Value.keyUpCommand);
            }
        }

        var moveInput = Gamepad.current.leftStick.ReadValue();
        if (moveInput.x > 0.2f)
        {
            Press(Executors[Define.GameKey.RightMove].keyCommand);
        }
        else if (moveInput.x < -0.2f)
        {
            Press(Executors[Define.GameKey.LeftMove].keyCommand);
        }
        else if (moveInput.y > 0.1f)
        {
            Press(Executors[Define.GameKey.Up].keyCommand);
        }
        else if (moveInput.y < -0.1f)
        {
            Press(Executors[Define.GameKey.Down].keyCommand);
        }
        else if (!InputManager.AnyButton())
        {
            inputIdle?.Invoke(user);
            BufferSetDirection(0);
        }

        buffer?.Routine();
    }


    public void ChangeCommands(CommandComp comp)
    {
        Executors.Keys.ForEach(x =>
        {
            Executors[x].keyDownCommand.Commands = comp.Commands[x].keyDownCommand;
            Executors[x].keyUpCommand.Commands = comp.Commands[x].keyUpCommand;
            Executors[x].keyCommand.Commands = comp.Commands[x].keyCommand;
        });
        inputEmptyMove = comp.inputEmptyMove;
        inputIdle = comp.inputIdle;
    }

    public void ReturnToBase()
    {
        ChangeCommands(baseCommands);
    }

    public void BufferSetDirection(EActorDirection direction)
    {
        buffer.SetDirection(direction);
    }

    public void BufferOrInvoke(CommandExecutor.CommandInfo command)
    {
        if (command == null) return;

        if (command.IsBufferAction) buffer.AddEvent(command);

        else if (buffer.InvokeCondition(command)) command.Invoke(user);
    }

    private void Press(CommandExecutor.CommandInfo commandInfo)
    {
        BufferOrInvoke(commandInfo);

        commandInfo.pressedAction?.Invoke();
    }

    private void KeyDown(Define.GameKey key)
    {
        pressedKeys.TryAdd(key, true);

        pressedKeys[key] = true;
    }

    private bool KeyUp(Define.GameKey key)
    {
        var value = false;
        if (pressedKeys.ContainsKey(key))
        {
            value = pressedKeys[key];
            pressedKeys[key] = false;
        }

        return value;
    }

    protected KeyCode GetKeyCode(Define.GameKey gameKey)
    {
        return KeySettingManager.GetGameKeyCode(gameKey);
    }

    public void PressDown(bool down = true)
    {
        if (down)
            _IsPressingDown.Value = true;
        else
            _IsPressingDown.CoyoteSet(false);
    }

    public void SetCommandState(ECommandType type)
    {
        CurrentCommand = type;
    }

    public void EnableControl()
    {
        isEnabled = true;
    }

    public void DisableControl()
    {
        isEnabled = false;
        buffer.ClearBuffer();
    }

    public bool IsPressing(Define.GameKey gameKey)
    {
        if (!pressedKeys.TryGetValue(gameKey, out var value)) return false;
        return value;
    }
}