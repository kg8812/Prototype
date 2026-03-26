using System;
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

    public UnityEvent<Player> OnPlayerCreated => _onPlayerCreated ??= new UnityEvent<Player>();
    public UnityEvent<Player> OnPlayerDie => _onPlayerDie ??= new UnityEvent<Player>();
    public UnityEvent<Player> onPlayerChange => _onPlayerChange ??= new UnityEvent<Player>();

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

                Progress.Exp += info.target.Exp;
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