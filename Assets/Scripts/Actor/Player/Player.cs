using System.Collections.Generic;
using Apis;
using Apis.Managers;
using Default;
using DG.Tweening;
using PlayerState;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public enum EPlayerState
{
    Idle,
    Move,
    Run,
    Jump,
    Attack,
    Skill,
    Dash,
    Damaged,
    Dead,
    Interact
}

public enum PlayerType
{
    Player1
}

public partial class Player : IDashUser, IMovable, IPlayer
{
    [Title("플레이어")] [SerializeField] private PlayerType _playerType;
    public Transform transForCamGroup;

    [HideInInspector] public AttackObject[] attackColliders;

    [HideInInspector] public PlayerResister resister;

    public ProjectileInfo atkInfo;

    public bool isStarted;

    private readonly List<Collider2D> _interactionColliders = new();

    private PlayerMoveComponent _moveComponent;

    private PlayerEffector effector;

    private bool[] pressingLR;
    public PlayerType playerType => _playerType;
    public ActorController Controller { get; private set; }

    public PlayerAnimator AnimController { get; private set; }

    public override Animator animator => AnimController.Animator;
    public override EffectSpawner EffectSpawner => Effector;
    public PlayerEffector Effector => effector ??= new PlayerEffector(this);

    public bool IsIdleFixed { get; private set; }
    
    public bool[] PressingLR
    {
        get
        {
            pressingLR ??= new[] { false, false };
            return pressingLR;
        }
    }

    public int InteractionColliderNum => _interactionColliders.Count;
    public IPlayerAttack attackStrategy;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        GameManager.Scene.WhenSceneLoaded.AddListener(s => { _interactionColliders.Clear(); });
        AnimController = GetComponent<PlayerAnimator>();
        attackStrategy = new PlayerBasicAttack(this);
        Controller = GetComponent<ActorController>();
        // animator = transform.GetChild(0).GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        attackColliders = GetComponentsInChildren<AttackObject>(true);
        name = "Player";

        effector ??= new PlayerEffector(this);

        animator.keepAnimatorStateOnDisable = true;
        resister = GetComponentInChildren<PlayerResister>();
        
        // 플레이어 자세 교정 -> scene Load event 등록
        GameManager.Scene.WhenSceneLoaded.AddListener(CorrectingPlayerPostureAction);
        AddEvent(EventType.OnDestroy,
            _ => GameManager.Scene.WhenSceneLoaded.RemoveListener(CorrectingPlayerPostureAction));

        attackColliders.ForEach(x =>
        {
            x.AddEvent(EventType.OnAttackSuccess, info =>
            {
                ExecuteEvent(EventType.OnColliderAttack, info);
            });
        });

        CoolDown = new PlayerCd(this);
        CoyoteCurrentJump = new CoyoteVar<int>(jumpCoyoteTime);
        MaxDropVel = initMaxDropVel;
        BonusStatEvent += LevelBonusStat;

        // start에 있던거 서순때문에 awake로 옮김


        StateInit();

        ActorMovement.dirVec = Vector2.right;
        StateMachineInit();
        _physicsTransitionHandler ??= gameObject.GetOrAddComponent<ActorPhysicsTransitionHandler>();
    }

    protected override void Start()
    {
        base.Start();
        // awake 구분 원래 자리

        if (GameManager.instance.Player == this) GameManager.instance.afterPlayerStart.Invoke(this);

        GameManager.instance.playerDied = false;
        isStarted = true;
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.Update();
        //BlockIdle = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        StateMachine.FixedUpdate();
        StateMachine.SubRoutine();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // TODO: Interaction 서치에 대해 현재 상태가 NonBattleState인가? 판단해야 함. (GameManager.instance.CurGameStateType
        // 방식이 현재 실시간 체크가 아니라 enter, exit로 판별하기 때문에 _interactionColliders 계산은 하지만 OnActivate는 안하는 방향으로

        if (collision.CompareTag("Interaction"))
            if (!_interactionColliders.Contains(collision))
            {
                if (_interactionColliders.Count > 0)
                {
                    var interaction = _interactionColliders[_interactionColliders.Count - 1]
                        .GetComponentInChildren<Interaction>();

                    interaction.OffActive();
                }

                _interactionColliders.Add(collision);

                var interaction2 = _interactionColliders[_interactionColliders.Count - 1]
                    .GetComponentInChildren<Interaction>();

                interaction2.OnActive();
            }
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);

        // TODO: 마찬가지로 NonBattleState에 대해 판별 후, OnActive만 안하는 방향으로 (Remove는 해야함)
        // TODO: _interactionColliders에 추가하였지만 Battle State에 추가했어서 다시 NonBattleState로 돌아왔을때 interactionColliders의 마지막 요소를 OnActive해줘야 함.
        if (_interactionColliders.Contains(collision))
        {
            if (_interactionColliders[_interactionColliders.Count - 1].Equals(collision))
            {
                var interaction = _interactionColliders[_interactionColliders.Count - 1]
                    .GetComponentInChildren<Interaction>();

                interaction.OffActive();

                if (_interactionColliders.Count > 1)
                {
                    var interaction2 = _interactionColliders[_interactionColliders.Count - 2]
                        .GetComponentInChildren<Interaction>();

                    interaction2.OnActive();
                }
            }

            _interactionColliders.Remove(collision);
        }
    }

    public UnitMoveComponent MoveComponent
    {
        get
        {
            if (_moveComponent == null)
            {
                _moveComponent = gameObject.GetOrAddComponent<PlayerMoveComponent>();
                _moveComponent.Init(this, Collider);
            }

            return _moveComponent;
        }
    }

    public void Stop()
    {
        MoveComponent?.Stop();
    }

    // 포탈 등 이동했을 때 자세 교정
    public virtual void CorrectingPlayerPosture(bool isLanding = true /* 바닥에 맞게 할지 여부 */)
    {
        Rb.linearVelocity = Vector2.zero;

        if (isLanding) MoveToFloor();

        MoveComponent.ActorMovement.Tweener.Kill();
        CancelAttack();
        IdleOn();
        // TODO: 플레이이 애니메이션 초기화..? 대쉬중 포탈들어갈때도 있어서 그거 idle로 바꿔줘야 함
    }

    public void IdleFixOn() // Idle 고정 함수
    {
        IsIdleFixed = true;
        ControlOff();
        IdleOn();
        AnimController.SetTrigger(EAnimationTrigger.IdleFix);
    }

    public void IdleFixOff() // Idle 고정 해제
    {
        IsIdleFixed = false;
        AnimController.SetTrigger(EAnimationTrigger.IdleFixOff);
        ControlOn();
        IdleOn();
    }

    public override void IdleOn()
    {
        if (!onAir) SetState(EPlayerState.Idle);
    }

    public void Dash(float distance, float time)
    {
        var tempDist = DashSpeed;
        var tempTime = DashTime;
        playerStat.dashSpeed = distance;
        playerStat.dashTime = time;
        SetState(EPlayerState.Dash);

        playerStat.dashSpeed = tempDist;
        playerStat.dashTime = tempTime;
    }


    private void UpdateSkills()
    {
        if (ActiveSkill != null)
        {
            ActiveSkill.Init();
            ActiveSkill = Instantiate(ActiveSkill);
            ActiveSkill.Init();

            ActiveSkill.Equip(this);
            _baseActiveSkill = ActiveSkill;
        }

        if (PassiveSkill != null)
        {
            PassiveSkill.Init();
            PassiveSkill = Instantiate(PassiveSkill);
            PassiveSkill.Init();
            PassiveSkill.Equip(this);
            _basePassiveSkill = PassiveSkill;
        }
    }

    public override void AttackOff()
    {
        ableAttack = false;
        SetAbleState(EPlayerState.Attack, false);
    }

    public override void AttackOn()
    {
        ableAttack = true;
        SetAbleState(EPlayerState.Attack);
        if (StateMachine?.CurrentState is BaseState s)
            s.AbleStates.Add(EPlayerState.Attack);
    }

    public void PressLR(EActorDirection dir, bool value = true)
    {
        // pressingLR[0] : left, pressingLR[1]: right 
        if (pressingLR == null) pressingLR ??= new[] { false, false };
        var idx = dir == EActorDirection.Left ? 0 : 1;
        pressingLR[idx] = value;
    }

    public void ChangeToIdle()
    {
        SetState(EPlayerState.Idle);
    }

    public override void Die()
    {
        base.Die();
        SetState(EPlayerState.Dead);
        animator.SetTrigger("Dead");
        ActiveSkill?.Cancel();
        PassiveSkill?.Cancel();

        DOTween.Sequence().SetDelay(3f).OnComplete(() => { GameManager.instance.GameOver(); });
    }

    public IOnInteract getInteract()
    {
        if (InteractionColliderNum <= 0) return null;

        var ionInteract =
            _interactionColliders[InteractionColliderNum - 1].gameObject.GetComponent<IOnInteract>();
        ionInteract ??= _interactionColliders[InteractionColliderNum - 1].transform.parent
            .GetComponentInChildren<IOnInteract>();

        return ionInteract;
    }

    public void StopMoving()
    {
        if (onAir)
            MoveComponent.ActorMovement.StopWithFall();
        else
            MoveComponent.ActorMovement.Stop();
    }

    public void Step()
    {
        MoveComponent.ActorMovement.StepMove();
    }

    public override void SetDirection(EActorDirection input)
    {
        animator.SetInteger("direction", (int)input);
        if (Direction != input)
        {
            Rb.linearVelocity = new Vector2(0, Rb.linearVelocity.y);
            base.SetDirection(input);
        }

        Controller.BufferSetDirection(input);
    }


    // removeListener를 위한 action 지정
    private void CorrectingPlayerPostureAction(SceneData _)
    {
        CorrectingPlayerPosture();
    }

    public Vector2 GetSlope()
    {
        return MoveComponent.ActorMovement.GetSlopeVetor();
    }
}