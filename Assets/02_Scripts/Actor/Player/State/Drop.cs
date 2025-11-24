using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.CommonMonster2;
using UnityEngine;

namespace PlayerState {
    public class Drop : EventState, IAnimate
    {
        public override EPlayerState NextState { get => EPlayerState.AirIdle; set{} }
        private Collider2D platform;

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            platform = _player.DropStart();

            if(platform == null) {
                _player.SetState(EPlayerState.Idle);
                return;
            }

            _player.IsFixGravity = true;
            _player.GravityOn();

            _player.StateEvent.AddEvent(EventType.OnLanding, (e)=>_player.DropOver(platform));

            _player.StateEvent.AddEvent(EventType.OnEventState, (e) => _player.DropOver(platform));
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.IsFixGravity = false;
        }

        public override bool EscapeCondition()
        {
            return _player.onAir;
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.OnAir, true);
            _player.AnimController.SetTrigger(EAnimationTrigger.Drop);
        }

        public void OnExitAnimate()
        {
            
        }
    }
}