using Apis;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Apis;
using Apis.Managers;
using Default;
using Save.Schema;
using Sirenix.Utilities;
using Spine;
using UnityEngine;
using PlayerState;
using Spine.Unity;

public enum EPlayerState
{
    Idle,
    Move,
    Run,
    Jump,
    Fall,
    Attack,
    Skill,
    Dash,
    Damaged,
    Dead,
    Interact,
    KnockBack,
    KnockBackEnd,
}

public enum PlayerType
{
    Player1,
    
}
public partial class Player : IDashUser , IMovable , IPlayer
{
    [Title("플레이어")] [SerializeField] private PlayerType _playerType;
    public PlayerType playerType => _playerType;
    List<Collider2D> _interactionColliders = new List<Collider2D>();
    public Transform transForCamGroup;

    ActorController controller;
    public ActorController Controller => controller;
    CapsuleCollider2D playerCollisionCollider;
    public CapsuleCollider2D PlayerCollisionCollider => playerCollisionCollider;

    private PlayerAnimator _animator;
    public PlayerAnimator AnimController => _animator;
    public override Animator animator => _animator.Animator;
    
    PlayerEffector effector;
    public override EffectSpawner EffectSpawner => Effector;
    public PlayerEffector Effector => effector ??= new(this);

    [HideInInspector] public AttackObject[] attackColliders;

    [HideInInspector] public PlayerResister resister;
    
    public bool IsIdleFixed { get; private set; }

    protected override TextShow DmgText => null;

    public ProjectileInfo atkInfo;

    [LabelText("크리스탈 offset")] public Vector2 nunnaCrystalOffset;
    private PetFollower nunnaCrystal;
   
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
        if(!onAir) SetState(EPlayerState.Idle);
    }
    
    public void UsePotion()
    {
        if (CurrentPotionCapacity > 0)
        {
            increasePotionCapacity(-1);
            float value = CalculateRepair();
            CurHp += value;
            EventParameters parameters = new(this);
            ExecuteEvent(EventType.OnRepair, parameters);
        }
    }

    public float CalculateRepair()
    {
        return (MaxHp * (playerStat.potionIncreaseHpRatio / 100) + playerStat.potionIncreaseHp) * (1 + repairRatio / 100);
    }

    public void Dash(float distance, float time)
    {
        float tempDist = DashSpeed;
        float tempTime = DashTime;
        playerStat.dashSpeed = distance;
        playerStat.dashTime = time;
        SetState(EPlayerState.Dash);
        
        playerStat.dashSpeed = tempDist;
        playerStat.dashTime = tempTime;
    }

    public override void StartStun(IEventUser actor, float duration)
    {
        base.StartStun(actor, duration);
        SubBuffManager.AddCC(actor,SubBuffType.Debuff_Stun,duration);
        ControlOff(true);
    }

    public override void EndStun()
    {
        base.EndStun();
        ControlOn();
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
        GameManager.Scene.WhenSceneLoaded.AddListener((s) => { _interactionColliders.Clear(); });
        _animator = GetComponent<PlayerAnimator>();
        attackStrategy = new PlayerWeaponAttack(this);
        controller = GetComponent<ActorController>();
        playerCollisionCollider = transform.GetChild(5).GetComponent<CapsuleCollider2D>();
        // animator = transform.GetChild(0).GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        
        BonusStatEvent += () => InvenManager.instance.Acc.BonusStat;
        attackColliders = GetComponentsInChildren<AttackObject>(true);
        name = "Player";

        effector = new PlayerEffector(this);

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
                ExecuteEvent(EventType.OnColliderAttack,info);
                if (AttackItemManager.CurrentItem is Weapon weapon && info?.target is Actor)
                {
                    weapon.SFXPlayer?.Init(this);
                    weapon.SFXPlayer?.Play();
                }
            });
        });
        
        int index = animator.GetLayerIndex("BeastLayer");
        animator.SetLayerWeight(index,0);
        CoolDown = new(this);
        CoyoteCurrentJump = new(jumpCoyoteTime, 0);
        maxDropVel = initMaxDropVel;
        BonusStatEvent += LevelBonusStat;
        nunnaCrystal = EffectSpawner
            .Spawn("NunnaCrystal", transform.position, false, true)
            .GetComponent<PetFollower>();
        
        nunnaCrystal.Init(transform, this, new Vector2(-nunnaCrystalOffset.x, nunnaCrystalOffset.y));
        
        // start에 있던거 서순때문에 awake로 옮김
        

        StateInit();

        ActorMovement.dirVec = Vector2.right;
        StateMachineInit();
        _physicsTransitionHandler ??= gameObject.GetOrAddComponent<ActorPhysicsTransitionHandler>();

    }

    public bool isStarted;
    protected override void Start()
    {
        base.Start();
        _overrider.Init(this);
        // awake 구분 원래 자리

        if (GameManager.instance.Player == this)
        {
            GameManager.instance.afterPlayerStart.Invoke(this);
        }
        
        GameManager.instance.playerDied = false;
        isStarted = true;
    }

    
    void UpdateSkills()
    {
        if (activeSkill != null)
        {
            activeSkill.Init();
            activeSkill = Instantiate(activeSkill);
            activeSkill.Init();

            activeSkill.Equip(this);
            _baseActiveSkill = activeSkill;
        }

        if (passiveSkill != null)
        {
            passiveSkill.Init();
            passiveSkill = Instantiate(passiveSkill);
            passiveSkill.Init();
            passiveSkill.Equip(this);
            _basePassiveSkill = passiveSkill;
        }
        
        GameManager.instance.OnPlayerDestroy.RemoveListener(UnEquipSkills);
        GameManager.instance.OnPlayerDestroy.AddListener(UnEquipSkills);
    }

    protected override void Update()
    {
        base.Update();
        _playerStateMachine.Update();
        //BlockIdle = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _playerStateMachine.FixedUpdate();
        _playerStateMachine.SubRoutine();
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
        if(_playerStateMachine?.CurrentState is BaseState s)
            s.AbleStates.Add(EPlayerState.Attack);
    }

    private PlayerMoveComponent _moveComponent;

    public UnitMoveComponent MoveComponent
    {
        get
        {
            if (_moveComponent == null)
            {
                _moveComponent = gameObject.GetOrAddComponent<PlayerMoveComponent>();
                _moveComponent.Init(this,Collider);
            }

            return _moveComponent;
        }
    }
    
    private bool[] pressingLR = null;
    public bool[] PressingLR{
        get{
            pressingLR ??= new bool[] { false, false };
            return pressingLR;
        }
    }
    public void PressLR(EActorDirection dir, bool value = true)
    {
        // pressingLR[0] : left, pressingLR[1]: right 
        if(pressingLR == null) pressingLR ??= new bool[] { false, false };
        int idx = dir == EActorDirection.Left ? 0 : 1;
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
        activeSkill?.Cancel();
        passiveSkill?.Cancel();

        DOTween.Sequence().SetDelay(3f).OnComplete(() =>
        {
            GameManager.instance.GameOver();
        });
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        // TODO: Interaction 서치에 대해 현재 상태가 NonBattleState인가? 판단해야 함. (GameManager.instance.CurGameStateType
        // 방식이 현재 실시간 체크가 아니라 enter, exit로 판별하기 때문에 _interactionColliders 계산은 하지만 OnActivate는 안하는 방향으로
        
        if (collision.CompareTag("Interaction"))
        {
            if (!_interactionColliders.Contains(collision))
            {
                if (_interactionColliders.Count > 0)
                {
                    Interaction interaction = _interactionColliders[_interactionColliders.Count - 1]
                        .GetComponentInChildren<Interaction>();

                    interaction.OffActive();
                }

                _interactionColliders.Add(collision);

                Interaction interaction2 = _interactionColliders[_interactionColliders.Count - 1]
                    .GetComponentInChildren<Interaction>();

                interaction2.OnActive();
            }
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
                Interaction interaction = _interactionColliders[_interactionColliders.Count - 1]
                    .GetComponentInChildren<Interaction>();

                interaction.OffActive();

                if (_interactionColliders.Count > 1)
                {
                    Interaction interaction2 = _interactionColliders[_interactionColliders.Count - 2]
                        .GetComponentInChildren<Interaction>();

                    interaction2.OnActive();
                }
            }

            _interactionColliders.Remove(collision);
        }
    }


    private void CheckInteractionCollider()
    {
    }

    public int InteractionColliderNum
    {
        get { return _interactionColliders.Count; }
    }

    public IOnInteract getInteract()
    {
        if (InteractionColliderNum <= 0) return null;

        IOnInteract ionInteract =
            _interactionColliders[InteractionColliderNum - 1].gameObject.GetComponent<IOnInteract>();
        ionInteract ??= _interactionColliders[InteractionColliderNum - 1].transform.parent
            .GetComponentInChildren<IOnInteract>();

        return ionInteract;
    }

    public void StopMoving()
    {
        if(onAir)
            MoveComponent.ActorMovement.StopWithFall();
        else
            MoveComponent.ActorMovement.Stop();
    }

    public void Stop()
    {
        MoveComponent?.Stop();
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
            Rb.linearVelocity = new Vector2(0,Rb.linearVelocity.y);
            base.SetDirection(input);
            AnimController.SetTrigger(EAnimationTrigger.Turn);
        }
        controller.BufferSetDirection(input);
    }

    public void StopJumping()
    {
        // animator.SetBool("OnAir", false);
        // currentJump = 0;
        // if (IsDrop) DropOver();
        // Rb.velocity = new Vector2(Rb.velocity.x, 0);
        // AirDashed = 0;
        // onAir = false;
    }

    // removeListener를 위한 action 지정
    private void CorrectingPlayerPostureAction(SceneData _) => CorrectingPlayerPosture();

    // 포탈 등 이동했을 때 자세 교정
    public virtual void CorrectingPlayerPosture(bool isLanding = true /* 바닥에 맞게 할지 여부 */)
    {
        Rb.linearVelocity = Vector2.zero;

        if (isLanding)
        {
            MoveToFloor();
        }

        MoveComponent.ActorMovement.Tweener.Kill();
        CancelAttack();
        IdleOn();
        // TODO: 플레이이 애니메이션 초기화..? 대쉬중 포탈들어갈때도 있어서 그거 idle로 바꿔줘야 함
    }

    public Vector2 GetSlope()
    {
        return MoveComponent.ActorMovement.GetSlopeVetor();
    }

}