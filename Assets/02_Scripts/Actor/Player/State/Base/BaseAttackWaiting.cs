using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

public class BaseAttackWaiting : EventState
{
    public override EPlayerState NextState { get => EPlayerState.Stop; set{} }
    private readonly EPlayerState[] afterState = new EPlayerState[] { EPlayerState.Move, EPlayerState.Jump, EPlayerState.Dash, EPlayerState.Heal, EPlayerState.Crouch };

    private bool escapeFlag = false;
    public override bool EscapeCondition()
    {
        // var anim = _player.animator.GetCurrentAnimatorStateInfo(0);
        // return anim.normalizedTime > 0.9999f;   // TODO: animator 리팩토링 후 변경
        return escapeFlag;
    }

    public override void OnEnter(Player t)
    {
        base.OnEnter(t);
        _player.OnAttack = true;

        escapeFlag = false;

        _player.Controller.SetCommandState(Command.ECommandType.Attack);

        _player.CoolDown.StartCd(EPlayerCd.AttackComboDelay, 
                                    () => { 
                                        AddAbleState(EPlayerState.Attack); 
                                    });  

        _player.CoolDown.StartCd(EPlayerCd.AttackAfterDelay,
                                () => {
                                    foreach(var state in afterState)
                                        AddAbleState(state);
                                });

        _player.StateEvent.AddEvent(EventType.OnIdleMotion, (e) => escapeFlag = true);
    }

    public override void OnExit()
    {
        base.OnExit();
        _player.OnAttack = false;
        _player.ExecuteEvent(EventType.OnAttackStateExit,null);
        _player.StateEvent.ExecuteEventOnce(EventType.OnAttackStateExit, null);
        _player.Controller.SetCommandState(Command.ECommandType.None);

        _player.CoolDown.StopCd(EPlayerCd.AttackAfterDelay);
        _player.CoolDown.StopCd(EPlayerCd.AttackComboDelay);
    }
}
