using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Command;
using DG.Tweening;
using PlayerState;
using UnityEngine;
using UnityEngine.Events;
using Dash = PlayerState.Dash;
using Idle = PlayerState.Idle;
using Interact = PlayerState.Interact;
using Jump = PlayerState.Jump;
using Move = PlayerState.Move;
using Skill = PlayerState.Skill;

public partial class Player : Actor
{
    private const float searchDepth = 0.1f;
    public bool PhysicTest;
    public uint MaxAirDash = 1;
    public bool StateLog;

    private readonly Dictionary<EPlayerState, bool> _AbleState = new();

    private UnityEvent _OnChargeEnd;

    private UnityEvent _onChargeStart;

    private readonly Dictionary<EPlayerState, IState<Player>> _playerStateDictionary = new();

    public CoyoteVar<int> CoyoteCurrentJump;

    private Tweener dashLandingTweener;

    public StateEvent StateEvent { get; } = new();

    public ECommandType CurrentCommand => Controller.CurrentCommand;

    public bool AbleDash { get; private set; }

    public bool IsCrouch { get; } = false;

    public bool IsDrop { get; private set; }

    public bool AbleAttack => GetAbleState(EPlayerState.Attack);

    public bool ableMove => MoveComponent.ableMove;
    public bool ableJump => MoveComponent.ableJump;
    public bool IsClimb { get; set; }
    public bool IsMove { get; set; }

    public bool OnAttack { get; set; }
    public bool OnFinalAttack { get; set; }
    public bool IsSkill { get; set; }
    public bool OnFinalSkill { get; set; }

    public int AirDashed { get; set; }
    public bool IsReadyIdle { get; set; }

    public bool isInteractable { get; set; }
    public EActorDirection PressingDir => Controller.PressingDir;
    public UnityEvent OnChargeStart => _onChargeStart ??= new UnityEvent();
    public UnityEvent OnChargeEnd => _OnChargeEnd ??= new UnityEvent();
    public bool IsFixGravity { get; set; }

    public PlayerStateMachine StateMachine { get; private set; }

    public EPlayerState CurrentState { get; private set; }

    public bool IsDash { get; set; }

    public virtual void DashOff()
    {
        AbleDash = false;
        SetAbleState(EPlayerState.Dash, false);
    }

    public virtual void DashOn()
    {
        AbleDash = true;
        SetAbleState(EPlayerState.Dash);
        if (StateMachine?.CurrentState is BaseState s)
            s.AbleStates.Add(EPlayerState.Dash);
    }

    public ActorMovement ActorMovement => MoveComponent?.ActorMovement;

    public void MoveOn()
    {
        MoveComponent.MoveOn();
        SetAbleState(EPlayerState.Move);

        if (StateMachine?.CurrentState is BaseState s) s.AbleStates.Add(EPlayerState.Move);
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
        SetAbleState(EPlayerState.Jump);
        if (StateMachine?.CurrentState is BaseState s)
            s.AbleStates.Add(EPlayerState.Jump);
    }

    public void JumpOff()
    {
        MoveComponent.JumpOff();
        SetAbleState(EPlayerState.Jump, false);
    }

    public void BlockIdle(bool isBlock = true)
    {
        SetAbleState(EPlayerState.Idle, isBlock);
    }

    private IEnumerator PlayerWaitCoroutine(UnityAction action)
    {
        IdleOn();
        yield return new WaitUntil(() =>
        {
            return !IsMove && !onAir && !OnAttack && !IsDash && !IsCrouch && !IsClimb && IsReadyIdle;
        });
        action.Invoke();
    }

    // 강제 상태 전환 포함 control on-off: 플레이어 밖에서 쓰는 경우에 로직 정확히 몰라서 그냥 유지지
    public void ControlOff(bool isBlockAttck = false)
    {
        MoveComponent.MoveCCOn();
        Controller.DisableControl();
        if (isBlockAttck) AttackOff();
    }

    public void ControlOn()
    {
        MoveComponent.MoveCCOff();
        AttackOn();
        Controller.EnableControl();
    }

    public void ControllerOn()
    {
        Controller.EnableControl();
    }

    public void ControllerOff()
    {
        Controller.DisableControl();
    }

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
        if (!IsDrop || platform == null) return;

        Physics2D.IgnoreCollision(PlayerCollisionCollider, platform, false);
        IsDrop = false;
    }

    public Collider2D DropStart()
    {
        if (!IsDropable(out var platform)) return null;

        Physics2D.IgnoreCollision(PlayerCollisionCollider, platform, true);

        IsDrop = true;

        return platform;
    }

    public bool IsDropable(out Collider2D platform)
    {
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector2.down, searchDepth, LayerMasks.Platform);

        platform = hit.collider;

        return !(hit.collider == null) && Controller.IsPressingDown;
    }

    public Tweener DashLanding(float time, float distance, Ease graph)
    {
        var tempSpeed = Rb.linearVelocity;
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
            animator.SetTrigger("CancelMotion");
        }
    }

    private void StateMachineInit()
    {
        MakeDict();
        StateMachine = new PlayerStateMachine(this, _playerStateDictionary[EPlayerState.Idle]);
    }

    public void SetState(EPlayerState state)
    {
        IState<Player> outState;
        if (_playerStateDictionary.TryGetValue(state, out outState))
        {
            if (StateMachine.CurrentState != outState)
            {
                CurrentState = state;
                if (StateLog) Debug.Log(state);
            }

            StateMachine.SetState(outState);
        }
    }

    public EPlayerState GetState()
    {
        foreach (var kv in _playerStateDictionary)
            if (kv.Value == StateMachine.CurrentState)
                return kv.Key;

        return EPlayerState.Idle;
    }

    private void MakeDict()
    {
        _playerStateDictionary.Add(EPlayerState.Idle, new Idle());
        _playerStateDictionary.Add(EPlayerState.Move, new Move());
        _playerStateDictionary.Add(EPlayerState.Jump, new Jump());
        _playerStateDictionary.Add(EPlayerState.Dash, new Dash());
        _playerStateDictionary.Add(EPlayerState.Attack, new Attack());
        _playerStateDictionary.Add(EPlayerState.Damaged, new Damaged());
        _playerStateDictionary.Add(EPlayerState.Dead, new Dead());
        _playerStateDictionary.Add(EPlayerState.Skill, new Skill());
        _playerStateDictionary.Add(EPlayerState.Interact, new Interact());
    }

    private void StateInit()
    {
        ableAttack = true;
        MoveComponent.ableMove = true;
        AbleDash = true;
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
            if (!_AbleState.TryAdd((EPlayerState)state, false))
                _AbleState[(EPlayerState)state] = false;

        _AbleState[EPlayerState.Idle] = true;

        StateEvent.AddEvent(EventType.OnIdle, e => { _AbleState[EPlayerState.Idle] = false; });
        CurrentState = EPlayerState.Idle;
    }

    public void SetAbleState(EPlayerState state, bool value = true)
    {
        _AbleState[state] = value;
    }

    public bool GetAbleState(EPlayerState state)
    {
        if (!_AbleState.TryGetValue(state, out var value))
            return false;

        return value;
    }

    public void SetDropMaxVel(float value)
    {
        MaxDropVel = value;
    }

    public void ResetDropResistFactor()
    {
        MaxDropVel = initMaxDropVel;
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