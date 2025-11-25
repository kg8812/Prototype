using System;
using Apis.DataType;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager
{
    [HideInInspector] public bool playerDied = false;

    [HideInInspector] public UnityEvent<Player> playerInit;
    [HideInInspector] public UnityEvent<Player> playerRegistered;
    private UnityEvent<Player> _onPlayerCreated;
    public UnityEvent<Player> OnPlayerCreated => _onPlayerCreated ??= new();
    [HideInInspector] public UnityEvent<Player> afterPlayerStart = new();
    private UnityEvent<Player> _onPlayerDie = new();
    public UnityEvent<Player> OnPlayerDie => _onPlayerDie ??= new();
    UnityEvent<Player> _onPlayerChange = new();
    public UnityEvent<Player> onPlayerChange => _onPlayerChange ??= new();

    public static Guid PlayerExistSceneStageGuid;

    [HideInInspector] public Action<int> levelChange;
    [HideInInspector] public Action<int> expChange;
    int level = 1;
    int exp = 0;

    public int Level
    {
        get => level;
        set
        {
            if (level <= 0)
            {
                level = 1;
            }
            else if (level > 100)
            {
                level = 100;
            }
            else
            {
                level = value;
            }
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
            LevelDataType levelData = LevelDatabase.GetLevelData(level);
            if (levelData == null)
            {
                expChange?.Invoke(exp);
                return;
            }
            
            int maxExp = levelData.exp;
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
    
   [SerializeField]
    [ReadOnly]
    private Player player;

    public Player Player
    {
        get => player;
        set
        {
            if(player != null && player != value)
            {
                Destroy(player.gameObject);
            }
            
            player = value;
            if (value == null)
            {
                playerTrans = null;
                PlayerController = null;
                return;
            }

            playerTrans = player.GetComponent<Transform>();
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

    private Transform playerTrans;

    public Transform PlayerTrans => playerTrans;

    private UnityEvent<Player> _onPlayerDestroy;
    public UnityEvent<Player> OnPlayerDestroy => _onPlayerDestroy ??= new();

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
