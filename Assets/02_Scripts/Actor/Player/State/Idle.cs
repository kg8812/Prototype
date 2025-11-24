using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace PlayerState {
    public class Idle : BaseGroundState, IAnimate
    {
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            _player.StateEvent.ExecuteEventOnce(EventType.OnIdle, null);

            _player.StateEvent.AddEvent(EventType.OnAirEnter, OnAir);
        }

        public override void FixedUpdate()
        {
            if(_player.IsIdleFixed) return;

            base.FixedUpdate();

            if(_player.PhysicTest)
            {
                _player.MoveComponent.ForceActorMovement.Friction(5);
            }
            _player.resister.Resist();
        }
        public override void OnExit()
        {
            base.OnExit();

            _player.StateEvent.RemoveEvent(EventType.OnAirEnter, OnAir);
        }

        private void OnAir(EventParameters e) => _player.SetState(EPlayerState.AirIdle);

        public void OnEnterAnimate()
        {
        }

        public void OnExitAnimate()
        {
        }
    }
}