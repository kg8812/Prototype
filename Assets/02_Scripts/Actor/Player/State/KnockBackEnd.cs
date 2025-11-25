using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

public class KnockBackEnd : EventState, IAnimate, IInterruptable
{
    public override EPlayerState NextState { get => EPlayerState.Idle; set{} }
    public float InterruptTime { get => _player.KnockbackEndCancelTime; set{} }

    public EPlayerState[] InteruptableStates => new EPlayerState[] { EPlayerState.Attack, EPlayerState.Skill, EPlayerState.Jump, EPlayerState.Dash, EPlayerState.Move,};

    public override bool EscapeCondition()
    {
        return escapeFlag;
    }
    private bool escapeFlag = false;
    public override void OnEnter(Player t)
    {
        base.OnEnter(t);
        if(_player.onAir) escapeFlag = true;

        _player.StateEvent.AddEvent(EventType.OnIdleMotion, (e) => escapeFlag = true );
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _player.MoveComponent.ForceActorMovement.Friction(5);
    }

    public override void OnExit()
    {
        base.OnExit();
        // Debug.Log("Exit");
        _player.StateEvent.ExecuteEventOnce(EventType.OnKnockbackComplete, null);
    }

    public void OnEnterAnimate()
    {
        _player.AnimController.ResetTrigger(EAnimationTrigger.KnockbackEnd);
        _player.AnimController.SetTrigger(EAnimationTrigger.KnockbackEnd);
    }

    public void OnExitAnimate()
    {
    }
}
