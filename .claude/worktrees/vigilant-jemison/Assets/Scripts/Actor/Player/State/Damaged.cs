using System;
using UnityEngine;

namespace PlayerState
{
    public class Damaged : BaseHitState, IAnimate
    {
        private Coroutine exitTimer;
        private Guid guid;

        public override EPlayerState NextState
        {
            get => base.NextState;
            set => base.NextState = value;
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.Damaged);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.IdleOn);
        }

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            guid = _player.AddInvincibility();

            _player.Stop();

            PhysicalEvent(eventParameters, data);

            exitTimer = _player.StartTimer(1, () => escapeFlag = true);
        }

        public override void OnExit()
        {
            base.OnExit();

            _player.StopTimer(exitTimer);
        }
    }
}