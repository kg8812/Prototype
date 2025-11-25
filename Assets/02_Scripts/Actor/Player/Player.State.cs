using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis;
using DG.Tweening;
using PlayerState;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public partial class Player : Actor
{
    public bool PhysicTest =false;

    Dictionary<EPlayerState, IState<Player>> _playerStateDictionary = new Dictionary<EPlayerState, IState<Player>>();
    readonly private StateEvent _StateEvent = new();
    public StateEvent StateEvent => _StateEvent;
    PlayerStateMachine _playerStateMachine;

    public Command.ECommandType CurrentCommand => controller.CurrentCommand;

    private Dictionary<EPlayerState, bool> _AbleState = new();

    public bool AbleDash => ableDash;
    private bool ableDash;

    public CoyoteVar<int> CoyoteCurrentJump;

    public bool IsCrouch => isCrouch;
    private bool isCrouch = false;

    public bool IsDrop => isDrop;
    private bool isDrop = false;

    public bool AbleAttack => GetAbleState(EPlayerState.Attack);
 
    public bool ableMove => MoveComponent.ableMove;
    public bool ableJump => MoveComponent.ableJump;
    public bool IsClimb { get; set; }
    public bool IsMove { get; set; }
    public bool IsDash { get; set; }

    public bool OnAttack { get; set; }
    public bool OnFinalAttack { get; set; }
    public bool IsSkill { get; set; }
    public bool OnFinalSkill { get; set; }

    public int AirDashed { get; set; }
    public bool IsReadyIdle { get; set; }
    public uint MaxAirDash = 1;

    public bool isInteractable { get; set; }
    public ActorMovement ActorMovement => MoveComponent?.ActorMovement;
    public EActorDirection PressingDir => Controller.PressingDir;
    
    UnityEvent _onChargeStart;
    public UnityEvent OnChargeStart => _onChargeStart ??= new();

    private UnityEvent _OnChargeEnd;
    public UnityEvent OnChargeEnd => _OnChargeEnd ??= new();

    public void MoveOn()
    {
        MoveComponent.MoveOn();
        SetAbleState(EPlayerState.Move, true);

        if(_playerStateMachine?.CurrentState is BaseState s){
            s.AbleStates.Add(EPlayerState.Move);
        }
    }

    public void MoveOff()
    {
        MoveComponent.MoveOff();
        SetAbleState(EPlayerState.Move, false);
    }

    public void MoveCCOn()
    {
        MoveComponent.MoveCCOn();
    }
    public void MoveCCOff()
    {
        MoveComponent.MoveCCOff();
    }

    public void JumpOn()
    {
        MoveComponent.JumpOn();
        SetAbleState(EPlayerState.Jump, true);
        if(_playerStateMachine?.CurrentState is BaseState s)
            s.AbleStates.Add(EPlayerState.Jump);
    }

    public void JumpOff()
    {
        MoveComponent.JumpOff();
        SetAbleState(EPlayerState.Jump, false);
    }

    public virtual void DashOff()
    {
        ableDash = false;
        SetAbleState(EPlayerState.Dash, false);
    }
    public virtual void DashOn()
    {
        ableDash = true;
        SetAbleState(EPlayerState.Dash, true);
        if(_playerStateMachine?.CurrentState is BaseState s)
            s.AbleStates.Add(EPlayerState.Dash);
    }

    public void BlockIdle(bool isBlock = true)
    {
        SetAbleState(EPlayerState.Idle, isBlock);
    }

    IEnumerator PlayerWaitCoroutine(UnityAction action)
    {
        IdleOn();
        yield return new WaitUntil(() => {
            return !IsMove && !onAir && !OnAttack && !IsDash && !IsCrouch && !IsClimb && IsReadyIdle;
        });
        action.Invoke();
    }

    // 강제 상태 전환 포함 control on-off: 플레이어 밖에서 쓰는 경우에 로직 정확히 몰라서 그냥 유지지
    public void ControlOff(bool isBlockAttck = false)
    {
        MoveComponent.MoveCCOn();
        controller.DisableControl();
        if(isBlockAttck) AttackOff();
    }
    public void ControlOn()
    {
        MoveComponent.MoveCCOff();
        AttackOn();
        controller.EnableControl();
    }

    public void ControllerOn()
    {
        controller.EnableControl();
    }
    public void ControllerOff()
    {
        controller.DisableControl();
    }

    private bool isFixGravity = false;
    public bool IsFixGravity { get => isFixGravity; set => isFixGravity = value; }

    public void GravityOn()
    {
        // if(isGravityOn) return;
        ActorMovement.SetGravity();
    }

    public void SetGravity(float gravityScale)
    {
        // Debug.Log("Set gravity: " + gravityScale);
        ActorMovement.SetGravityScale(gravityScale);
        ActorMovement.SetGravity();
    }

    public void ResetGravity()
    {
        ActorMovement.ResetGravityScale();
        ActorMovement.SetGravity();
    }

    public void GravityOff()
    {
        // if(!isGravityOn) return;

        ActorMovement.SetGravityToZero();
    }
    
    public void DropOver(Collider2D platform)
    {
        if(!isDrop || platform == null) return;

        Physics2D.IgnoreCollision(playerCollisionCollider, platform, false);
        isDrop = false;
    }

    private const float searchDepth = 0.1f;
    public Collider2D DropStart()
    {
        if(!IsDropable(out var platform)) return null;

        Physics2D.IgnoreCollision(playerCollisionCollider, platform, true);

        isDrop = true;

        return platform;
    }

    public bool IsDropable(out Collider2D platform)
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.down, searchDepth, LayerMasks.Platform);

        platform = hit.collider;

        return !(hit.collider == null) && Controller.IsPressingDown;
    }

    private Tweener dashLandingTweener;
    public Tweener DashLanding(float time, float distance, DG.Tweening.Ease graph)
    {
        Vector2 tempSpeed = Rb.linearVelocity;
        Rb.linearVelocity = Vector2.zero;
        dashLandingTweener = ActorMovement.DashTemp(time, distance, false, graph);
        dashLandingTweener.onComplete += () => Rb.linearVelocity = tempSpeed; 
        return dashLandingTweener;
    }

    public void DashLandingOff()
    {
        dashLandingTweener?.Kill();
    }

    public void CancelAttack()
    {
        if (OnAttack)
        {
            weaponAtkInfo.atkCombo = 0;
            animator.SetTrigger("CancelMotion");
        }
    }
    
    private void StateMachineInit()
    {
        MakeDict();
        _playerStateMachine = new PlayerStateMachine(this, _playerStateDictionary[EPlayerState.Idle]);
    }

    public PlayerStateMachine StateMachine => _playerStateMachine;
    private EPlayerState _CurrentState;
    public EPlayerState CurrentState => _CurrentState;
    public bool StateLog = false;
    public void SetState(EPlayerState state)
    {
        IState<Player> outState;
        if (_playerStateDictionary.TryGetValue(state, out outState))
        {
            if(_playerStateMachine.CurrentState != outState){
                _CurrentState = state;
                if(StateLog) Debug.Log(state);
            }
            _playerStateMachine.SetState(outState);
        }
    }

    public EPlayerState GetState()
    {
        foreach(KeyValuePair<EPlayerState, IState<Player>> kv in _playerStateDictionary)
        {
            if(kv.Value == _playerStateMachine.CurrentState) return kv.Key;
        }

        return EPlayerState.Idle;
    }
    void MakeDict()
    {
        _playerStateDictionary.Add(EPlayerState.Idle, new Idle());
        _playerStateDictionary.Add(EPlayerState.Move, new Move());
        _playerStateDictionary.Add(EPlayerState.Jump, new Jump());
        _playerStateDictionary.Add(EPlayerState.Dash, new Dash());
        _playerStateDictionary.Add(EPlayerState.Attack, new Attack());
        _playerStateDictionary.Add(EPlayerState.Damaged, new Damaged());
        _playerStateDictionary.Add(EPlayerState.Dead, new Dead());
        _playerStateDictionary.Add(EPlayerState.Skill,new PlayerState.Skill());
        _playerStateDictionary.Add(EPlayerState.KnockBack, new KnockBack());
        _playerStateDictionary.Add(EPlayerState.KnockBackEnd, new KnockBackEnd());
        _playerStateDictionary.Add(EPlayerState.Interact, new Interact());
    }

    private void StateInit()
    {
        ableAttack = true;
        MoveComponent.ableMove = true;
        ableDash = true;
        onAir = false;
        AirDashed = 0;
        IsReadyIdle = true;

        IsClimb = false;
        IsMove = false;
        OnAttack = false;
        OnFinalAttack = false;
        IsSkill = false;
        OnFinalSkill = false;
        isInteractable = true;

        foreach (var state in Enum.GetValues(typeof(EPlayerState)))
        {
            if(!_AbleState.TryAdd((EPlayerState)state, false))
            {
                _AbleState[(EPlayerState)state] = false;
            }
        }
        _AbleState[EPlayerState.Idle] = true;

        StateEvent.AddEvent(EventType.OnIdle, (e) =>{
            _AbleState[EPlayerState.Idle] = false;
        });
        _CurrentState = EPlayerState.Idle;
    }

    public void SetAbleState(EPlayerState state, bool value = true)
    {
        _AbleState[state] = value;
    }

    public bool GetAbleState(EPlayerState state)
    {
        if(!_AbleState.TryGetValue(state, out bool value)) 
            return false;

        return value;
    }

    public void SetDropMaxVel(float value)
    {
        maxDropVel = value;
    }

    public void ResetDropResistFactor()
    {
        maxDropVel = initMaxDropVel;
    }

    public void ForcedResetState()
    {
        Stop();
        ResetGravity();
        GravityOn();
        StateInit();
        SetState(EPlayerState.Idle);
        AnimController.SetTrigger(EAnimationTrigger.IdleOn);
    }
}
