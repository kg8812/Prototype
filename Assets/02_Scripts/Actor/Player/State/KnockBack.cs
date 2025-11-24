using System;
using System.Collections;
using System.Collections.Generic;
using EventData;
using UnityEngine;

namespace PlayerState{
    public class KnockBack : BaseHitState, IAnimate
    {
        public override EPlayerState NextState { get => EPlayerState.KnockBackEnd; set => base.NextState = value; }
        private Coroutine exitTimer;
        Guid guid;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            // Debug.Break();
            // Debug.Log("knockback");
            _player.Stop();

            exitTimer = _player.StartTimer(_player.KnockbackDuration, ()=> escapeFlag = true);

            guid = _player.AddInvincibility();

            _player.StateEvent.AddEvent(EventType.OnKnockbackComplete, (e) => _player.RemoveInvincibility(guid));

            _player.StateEvent.AddEvent(EventType.OnLanding, (e) => escapeFlag = true );

            PhysicalEvent(eventParameters, data);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            /* On Landing 타이밍 놓쳤을 경우 대비 */
            // if(!_player.onAir) escapeFlag = true;
        }

        public void OnEnterAnimate()
        {
            Vector2 knockBackSrc = data.directionType == KnockBackData.DirectionType.AktObjRelative
            ? eventParameters.user.Position
            : eventParameters.master.Position;

            Vector2 currentPos = _player.transform.position;

            float dir = Mathf.Sign(knockBackSrc.x - currentPos.x);

            _player.AnimController.SetBool(EAnimationBool.IsFrontKnockback, dir * (float)_player.Direction >= 0);

            _player.AnimController.ResetTrigger(EAnimationTrigger.KnockbackEnter);
            _player.AnimController.SetTrigger(EAnimationTrigger.KnockbackEnter);
        }

        public override void OnExit()
        {
            base.OnExit();

            _player.Invincibility(false);

            _player.StopTimer(exitTimer);
        }

        public void OnExitAnimate()
        {

        }
    }
}