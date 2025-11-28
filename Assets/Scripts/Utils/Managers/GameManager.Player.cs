using System;
using Apis.DataType;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager
{
    public static Guid PlayerExistSceneStageGuid;
    [HideInInspector] public bool playerDied;

    [HideInInspector] public UnityEvent<Player> playerInit;
    [HideInInspector] public UnityEvent<Player> playerRegistered;
    [HideInInspector] public UnityEvent<Player> afterPlayerStart = new();

    [SerializeField] [ReadOnly] private Player player;

    private UnityEvent<Player> _onPlayerChange = new();
    private UnityEvent<Player> _onPlayerCreated;

    private UnityEvent<Player> _onPlayerDestroy;
    private UnityEvent<Player> _onPlayerDie = new();
    private int exp;
    public Action<int> expChange;
    private int level = 1;

    public Action<int> levelChange;

    public UnityEvent<Player> OnPlayerCreated => _onPlayerCreated ??= new UnityEvent<Player>();
    public UnityEvent<Player> OnPlayerDie => _onPlayerDie ??= new UnityEvent<Player>();
    public UnityEvent<Player> onPlayerChange => _onPlayerChange ??= new UnityEvent<Player>();

    public int Level
    {
        get => level;
        set
        {
            if (level <= 0)
                level = 1;
            else if (level > 100)
                level = 100;
            else
                level = value;
            levelChange?.Invoke(level);
        }
    }

    public int Exp
    {
        get => exp;
        set
        {
            if (value < 0) return;

            exp = value;
            var levelData = LevelDatabase.GetLevelData(level);
            if (levelData == null)
            {
                expChange?.Invoke(exp);
                return;
            }

            var maxExp = levelData.exp;
            while (exp >= maxExp)
            {
                exp -= maxExp;
                Level++;
                levelData = LevelDatabase.GetLevelData(Level);
                if (levelData == null) break;
                maxExp = levelData.exp;
            }

            expChange?.Invoke(exp);
        }
    }

    public Player Player
    {
        get => player;
        set
        {
            if (player != null && player != value) Destroy(player.gameObject);

            player = value;
            if (value == null)
            {
                PlayerTrans = null;
                PlayerController = null;
                return;
            }

            PlayerTrans = player.GetComponent<Transform>();
            PlayerController = player.GetComponent<ActorController>();

            playerRegistered.Invoke(player);

            //TODO Init을 Registered 뒤로 위치 옮겼음. 후에 문제 생길시 얘기하기

            if (!isInit)
            {
                playerInit.Invoke(player);
                isInit = true;
            }


            player.AddEvent(EventType.OnKill, info =>
            {
                if (info?.target is null or { IsDead: true }) return;

                Exp += info.target.Exp;
            });
            // CameraManager.instance.PlayerCam.Follow = playerTrans;
            player.AddEvent(EventType.OnDeath, _ => OnPlayerDie.Invoke(player));
            ChangeControllingEntity(player);
        }
    }

    public Actor ControllingEntity { get; private set; }

    public Transform PlayerTrans { get; private set; }

    public UnityEvent<Player> OnPlayerDestroy => _onPlayerDestroy ??= new UnityEvent<Player>();

    public void ChangeControllingEntity(Actor actor)
    {
        ControllingEntity = actor;
    }

    public void DestroyPlayer()
    {
        if (Player == null) return;
        OnPlayerDestroy?.Invoke(Player);
        Destroy(Player.gameObject);
        Player = null;
    }


    #region Player Util

    public void InitWithPlayer(Action<Player> action)
    {
        if (Player != null)
        {
            action.Invoke(Player);
        }
        else
        {
            void InvokeAction(Player p)
            {
                action.Invoke(p);
                playerRegistered.RemoveListener(InvokeAction);
            }

            playerRegistered.AddListener(InvokeAction);
        }
    }

    #endregion
}