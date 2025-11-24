using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace PlayerState {
    public class AirIdle : BaseAirState, IAnimate
    {
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            _player.StateEvent.ExecuteEventOnce(EventType.OnIdle, null);

            _player.StateEvent.AddEvent(EventType.OnLanding, OnLandingEvent);
        }

        public override void FixedUpdate()
        {
            if(_player.IsIdleFixed) return;
            
            base.FixedUpdate();

            _player.MoveComponent.ForceActorMovement.AirStop(_player.AirStopResistFactor);
        }
        public override void OnExit()
        {
            base.OnExit();

            _player.StateEvent.RemoveEvent(EventType.OnLanding, OnLandingEvent);
        }

        public override void Update()
        {
        }

        private void OnLandingEvent(EventParameters p)
        {
            _player.SetState(EPlayerState.Idle);
        }

        public void OnEnterAnimate()
        {
        }

        public void OnExitAnimate()
        {
        }
    }
}
