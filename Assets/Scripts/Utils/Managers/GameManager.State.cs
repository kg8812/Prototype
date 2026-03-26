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
    public enum GameStateType
    {
        DefaultState,
        InteractionState,
        BattleState,
        NonBattleState
    }

    public enum InputType
    {
        KeyBoard,
        GamePad
    }

    public abstract class GameState
    {
        protected readonly GameStateType myType;

        // public virtual void CheckState()
        // {
        //     // state 진입 조건 체크 전, 우선순위 판별
        //     if ((int)myType > (int)(GameManager.instance.CurGameStateType))
        //     {
        //         return;
        //     }
        // }

        public abstract void OnEnterState();

        public abstract void OnExitState();
        // public abstract void StateOn();
        // public abstract void StateOff();
        //
        // public virtual void StateOnForce()
        // {
        //     GameManager.instance.ChangeGameState(myType);
        // }

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
    public UnityEvent<GameStateType> GameStateChangedTo;
    public BattleState BattleStateClass;


    public DefaultState DefaultStateClass;
    public InteractionState InteractionStateClass;
    public NonBattleState NonBattleStateClass;
    private HashSet<Guid> preventHashset;

    private Dictionary<GameStateType, bool> StateDict;
    private Dictionary<GameStateType, HashSet<Guid>> stateGuids;

    public static bool PreventControl { get; set; }

    // [HideInInspector] public GameStateType PreGameStateType { get; private set; }
    public GameStateType CurGameStateType { get; private set; }

    public GameState CurState
    {
        get
        {
            switch (CurGameStateType)
            {
                case GameStateType.DefaultState:
                    return DefaultStateClass;
                case GameStateType.InteractionState:
                    return InteractionStateClass;
                case GameStateType.BattleState:
                    return BattleStateClass;
                case GameStateType.NonBattleState:
                default:
                    return NonBattleStateClass;
            }
        }
    }

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

        preventHashset = new HashSet<Guid>();
        DefaultStateClass = new DefaultState();
        InteractionStateClass = new InteractionState();
        BattleStateClass = new BattleState();
        NonBattleStateClass = new NonBattleState();

        GameStateChangedTo = new UnityEvent<GameStateType>();


        StateDict = new Dictionary<GameStateType, bool>();
        StateDict.Add(GameStateType.DefaultState, false);
        StateDict.Add(GameStateType.InteractionState, false);
        StateDict.Add(GameStateType.BattleState, false);
        StateDict.Add(GameStateType.NonBattleState, true);

        stateGuids = new Dictionary<GameStateType, HashSet<Guid>>();
        stateGuids.Add(GameStateType.DefaultState, new HashSet<Guid>());
        stateGuids.Add(GameStateType.InteractionState, new HashSet<Guid>());
        stateGuids.Add(GameStateType.BattleState, new HashSet<Guid>());
        stateGuids.Add(GameStateType.NonBattleState, new HashSet<Guid>());

        #endregion


        // PreGameStateType = GameStateType.NonBattleState;
        CurGameStateType = GameStateType.NonBattleState;
        PlayerExistSceneStageGuid = TryOnGameState(GameStateType.DefaultState);

        ToggleCountPlayTime(CurGameStateType);
        GameStateChangedTo.AddListener(ToggleCountPlayTime);

        Scene.WhenSceneLoaded.AddListener(sceneData =>
        {
            if (!sceneData.isPlayerMustExist)
            {
                if (PlayerExistSceneStageGuid == Guid.Empty)
                    PlayerExistSceneStageGuid = TryOnGameState(GameStateType.DefaultState);
            }
            else if (PlayerExistSceneStageGuid != Guid.Empty)
            {
                TryOffGameState(GameStateType.DefaultState, PlayerExistSceneStageGuid);
                PlayerExistSceneStageGuid = Guid.Empty;
            }
        });
        currentInputType = InputType.KeyBoard;
    }

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
        if (!preventHashset.Contains(guid)) return false;
        PreventControl = false;
        return preventHashset.Remove(guid) && preventHashset.Count <= 0;
    }

    private void SUpdate()
    {
        if (!PreventControl)
        {
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
    }

    /// <summary>
    ///     인수에 해당 하는 game state를 on.
    ///     해당 게임 스테이트가 현재 켜져있는 state보다 낮은 state라면(dict 기준 default, interaction, battle, nonbattle 순)
    ///     해당 gameState로 이전.
    /// </summary>
    public Guid TryOnGameState(GameStateType state)
    {
        // Debug.Log($"State tryon - {state}");
        var newGuid = Guid.NewGuid();
        stateGuids[state].Add(newGuid);
        StateDict[state] = true;
        CheckGameState();
        return newGuid;
    }


    /// <summary>
    ///     인수에 해당 하는 game state를 off.
    ///     해당 게임 스테이트가 현재 켜져있는 가장 낮은 state라면 다음으로 낮은 state로 이전.
    /// </summary>
    public void TryOffGameState(GameStateType state, Guid guid)
    {
        // Debug.Log($"State tryoff - {state}");
        if (stateGuids[state].Remove(guid) && stateGuids[state].Count <= 0)
        {
            StateDict[state] = false;
            CheckGameState();
        }
    }

    private void CheckGameState()
    {
        // StringBuilder sb = new("Check - ");
        foreach (var (key, value) in StateDict)
            // sb.Append($"{key}: {value} / ");
            if (value)
            {
                if (CurGameStateType != key)
                    // sb.Append($"changeTo: {key}");
                    ChangeGameState(key);

                // Debug.Log(sb);
                return;
            }

        // Debug.Log(sb);
    }

    // 강제 게임 상태 진입
    public void ChangeGameState(GameStateType toState)
    {
        if (toState == CurGameStateType) return;

        // 이전의 game state toggle은 강제로 off
        for (var i = 0; i < (int)toState; i++) StateDict[(GameStateType)i] = false;

        CurState.OnExitState();
        CurGameStateType = toState;
        CurState.OnEnterState();
        // Debug.LogError($"from {PreGameStateType} to {CurGameStateType}");
        GameStateChangedTo.Invoke(CurGameStateType);
    }
}