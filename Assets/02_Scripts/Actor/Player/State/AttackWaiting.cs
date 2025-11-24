using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState {
    public class AttackWaiting : BaseAttackWaiting, IInterruptable, IAnimate
    {

        public float InterruptTime { get => _player.AnimController.GetCurrentClipLength(0); set{} }
        public EPlayerState[] InteruptableStates { get => new[] { EPlayerState.Move, EPlayerState.Jump, EPlayerState.Dash, EPlayerState.Attack }; set{} }

        private readonly EPlayerState[] afterState = new EPlayerState[] { EPlayerState.Move, EPlayerState.Jump, EPlayerState.Dash, EPlayerState.Heal, EPlayerState.Crouch };
        private bool tmp;

        // private Coroutine interruptCoroutine;
        public override void OnEnter(Player t)
        { 
            base.OnEnter(t);

            tmp =_player.OnFinalAttack;

            _player.OnFinalAttack = false;

            _player.StateEvent.AddEvent(EventType.OnIdleMotion, (e)=>_player.weaponAtkInfo.atkCombo = 0);

            _player.StateEvent.AddEvent(EventType.OnEventState, (e)=>{ if(_player.CurrentState != EPlayerState.Attack) _player.weaponAtkInfo.atkCombo = 0; });
        }   
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.ResetGravity();
            if(tmp) _player.weaponAtkInfo.atkCombo = 0;
        }

        public void OnEnterAnimate()
        {
        }

        public void OnExitAnimate()
        {
            // _player.animController.Trigger(EAnimationTrigger.CancelMotion);
        }
    }
}
