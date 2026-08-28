using System;
using System.Collections.Generic;
using System.Linq;
using GameStateSpace;
using Save.Schema;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GameStateSpace
{
    public enum InputType
    {
        KeyBoard,
        GamePad
    }

    /// <summary>템플릿 기본 상태들의 우선순위. 게임 고유 상태는 이 사이 값을 골라 쓰면 된다.</summary>
    public static class StatePriority
    {
        /// <summary>UI만 조작 가능. 페이드 중, 시스템 알림 중, 플레이어가 없는 씬.</summary>
        public const int Default = 0;

        /// <summary>UI + 기본 조작. 상호작용 UI가 떠 있는 상태.</summary>
        public const int Interaction = 10;

        /// <summary>일반 플레이. 등록된 상태 중 우선순위가 가장 낮으므로 기본 상태가 된다.</summary>
        public const int Play = 100;
    }

    /// <summary>
    ///     게임 상태 하나. 게임마다 이 클래스를 상속해 상태를 만들고
    ///     <see cref="GameManager.RegisterState{T}" />로 등록한다.
    ///     동시에 여러 상태가 켜져 있을 수 있고, 그중 <see cref="Priority" />가 가장 작은 것이 활성 상태가 된다.
    /// </summary>
    public abstract class GameState
    {
        /// <summary>
        ///     낮을수록 우선. 등록된 상태 중 값이 가장 큰 것이
        ///     "아무 상태도 켜지지 않았을 때"의 기본 상태가 된다.
        /// </summary>
        public abstract int Priority { get; }

        public abstract void OnEnterState();

        public abstract void OnExitState();

        public virtual void KeyBoardControlling()
        {
            InputManager.ClearPushedKeycode();
        }

        public virtual void GamePadControlling()
        {
            InputManager.ClearPushedButtons();
        }
    }
}

public partial class GameManager : Singleton<GameManager>
{
    public UnityEvent<GameState> GameStateChangedTo;

    private HashSet<Guid> preventHashset;

    /// <summary>등록된 상태. Priority 오름차순으로 유지된다.</summary>
    private readonly List<GameState> _states = new();

    private readonly Dictionary<Type, GameState> _statesByType = new();

    /// <summary>
    ///     상태별로 "이 상태를 켜 둔 사람들"의 티켓. 개수가 0보다 크면 켜진 것이다.
    ///     on/off 플래그를 따로 두지 않는 이유: 장부가 둘이면 서로 어긋날 수 있기 때문.
    /// </summary>
    private readonly Dictionary<GameState, HashSet<Guid>> _stateGuids = new();

    public static bool PreventControl { get; set; }

    public GameState CurState { get; private set; }

    public InputType currentInputType { get; private set; }

    private void DetectInputType()
    {
        if (Gamepad.current != null && Gamepad.current.allControls.Any(x => x.IsPressed()))
        {
            DataAccess.Settings.Data.LoadGamePadImages();

            if (currentInputType == InputType.KeyBoard)
            {
                currentInputType = InputType.GamePad;
                DataAccess.Settings.Data.OnKeyChange?.Invoke();
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (Input.anyKeyDown)
        {
            if (currentInputType == InputType.GamePad)
            {
                currentInputType = InputType.KeyBoard;
                DataAccess.Settings.Data.OnKeyChange?.Invoke();
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void SAwake()
    {
        #region 변수 초기화

        // static이라 도메인 리로드를 끄면 이전 플레이의 값이 그대로 남는다. 반드시 초기화한다.
        PreventControl = false;
        preventHashset = new HashSet<Guid>();

        GameStateChangedTo = new UnityEvent<GameState>();

        _states.Clear();
        _statesByType.Clear();
        _stateGuids.Clear();

        // 템플릿 기본 상태. 게임 고유 상태는 RegisterState로 추가한다(GameManager.Sample.cs 참고).
        RegisterState(new DefaultState());
        RegisterState(new InteractionState());
        CurState = RegisterState(new PlayState());

        #endregion

        PlayerExistSceneStageGuid = TryOnGameState<DefaultState>();

        Scene.WhenSceneLoaded.AddListener(sceneData =>
        {
            if (!sceneData.isPlayerMustExist)
            {
                if (PlayerExistSceneStageGuid == Guid.Empty)
                    PlayerExistSceneStageGuid = TryOnGameState<DefaultState>();
            }
            else if (PlayerExistSceneStageGuid != Guid.Empty)
            {
                TryOffGameState<DefaultState>(PlayerExistSceneStageGuid);
                PlayerExistSceneStageGuid = Guid.Empty;
            }
        });
        currentInputType = InputType.KeyBoard;
    }

    #region 상태 등록 / 조회

    /// <summary>게임 상태를 등록한다. 같은 타입을 두 번 등록할 수는 없다.</summary>
    public T RegisterState<T>(T state) where T : GameState
    {
        if (state == null)
        {
            Debug.LogError("[GameManager] 등록하려는 GameState가 null이다.");
            return null;
        }

        var type = state.GetType();
        if (_statesByType.TryGetValue(type, out var exist))
        {
            Debug.LogError($"[GameManager] {type.Name}는 이미 등록되어 있다.");
            return (T)exist;
        }

        _statesByType.Add(type, state);
        _stateGuids.Add(state, new HashSet<Guid>());
        _states.Add(state);
        _states.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        return state;
    }

    /// <summary>등록된 상태 인스턴스를 가져온다. 등록되지 않았으면 null.</summary>
    public T GetState<T>() where T : GameState
    {
        if (_statesByType.TryGetValue(typeof(T), out var state)) return (T)state;

        Debug.LogError($"[GameManager] {typeof(T).Name}가 등록되지 않았다. RegisterState를 먼저 호출해야 한다.");
        return null;
    }

    /// <summary>현재 활성 상태가 T인가.</summary>
    public bool IsInState<T>() where T : GameState
    {
        return CurState is T;
    }

    #endregion

    public Guid PreventControlOn()
    {
        var guid = Guid.NewGuid();
        PreventControl = true;
        preventHashset.Add(guid);
        return guid;
    }

    /// <returns>prevent 해제되면 true 반환</returns>
    public bool PreventControlOff(Guid guid)
    {
        if (!preventHashset.Remove(guid)) return false;
        if (preventHashset.Count > 0) return false;

        PreventControl = false;
        return true;
    }

    private void SUpdate()
    {
        if (PreventControl || CurState == null) return;

        DetectInputType();
        switch (currentInputType)
        {
            case InputType.GamePad:
                CurState.GamePadControlling();
                break;
            case InputType.KeyBoard:
                CurState.KeyBoardControlling();
                break;
        }
    }

    #region 상태 on / off

    /// <summary>
    ///     해당 상태를 켠다. 반환된 guid를 들고 있다가 <see cref="TryOffGameState{T}" />에 넘겨 끈다.
    ///     켜진 상태 중 우선순위가 가장 높은(Priority가 작은) 것이 활성 상태가 된다.
    /// </summary>
    public Guid TryOnGameState<T>() where T : GameState
    {
        var state = GetState<T>();
        return state == null ? Guid.Empty : TryOnGameState(state);
    }

    public Guid TryOnGameState(GameState state)
    {
        if (state == null || !_stateGuids.TryGetValue(state, out var guids))
        {
            Debug.LogError($"[GameManager] 등록되지 않은 상태를 켜려 했다: {state}");
            return Guid.Empty;
        }

        var newGuid = Guid.NewGuid();
        guids.Add(newGuid);
        CheckGameState();
        return newGuid;
    }

    /// <summary>해당 상태를 끈다. 그 상태를 켜 둔 사람이 아무도 남지 않아야 실제로 꺼진다.</summary>
    public void TryOffGameState<T>(Guid guid) where T : GameState
    {
        var state = GetState<T>();
        if (state != null) TryOffGameState(state, guid);
    }

    public void TryOffGameState(GameState state, Guid guid)
    {
        if (state == null || !_stateGuids.TryGetValue(state, out var guids)) return;

        if (guids.Remove(guid) && guids.Count == 0) CheckGameState();
    }

    /// <summary>켜져 있는 상태들 중 우선순위가 가장 높은 것으로 옮긴다.</summary>
    private void CheckGameState()
    {
        GameState next = null;
        foreach (var state in _states)
            if (_stateGuids[state].Count > 0)
            {
                next = state;
                break;
            }

        // 아무것도 켜져 있지 않으면 우선순위가 가장 낮은 상태가 기본값이 된다.
        if (next == null && _states.Count > 0) next = _states[_states.Count - 1];

        if (next != null && next != CurState) ChangeGameState(next);
    }

    /// <summary>
    ///     강제로 상태를 바꾼다.
    ///     대상보다 우선순위가 높은(Priority가 작은) 상태들은 켜져 있어도 티켓을 전부 버린다.
    /// </summary>
    private void ChangeGameState(GameState toState)
    {
        if (toState == null || toState == CurState) return;
        
        CurState?.OnExitState();
        CurState = toState;
        CurState.OnEnterState();
        GameStateChangedTo.Invoke(CurState);
    }

    private void ChangeGameState<T>() where T : GameState
    {
        ChangeGameState(GetState<T>());
    }

    #endregion
}
