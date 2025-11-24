using System;
using System.Collections;
using System.Collections.Generic;
using EventData;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PlayerState {
    public class Damaged : BaseHitState, IAnimate
    {
        public override EPlayerState NextState { get => base.NextState; set => base.NextState = value; }
        private Coroutine exitTimer;
        Guid guid;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            
            guid = _player.AddInvincibility();

            _player.StartTimer(_player.StuckInvincibleDuration, () => _player.RemoveInvincibility(guid));

            _player.LookAtHitDirection(eventParameters);

            _player.Stop();

            PhysicalEvent(eventParameters, data);

            exitTimer = _player.StartTimer(_player.StuckDuration, () => escapeFlag = true );

        }

        public override void OnExit()
        {
            base.OnExit();
            
            _player.StopTimer(exitTimer);
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.Damaged);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.IdleOn);
        }
    }
}
