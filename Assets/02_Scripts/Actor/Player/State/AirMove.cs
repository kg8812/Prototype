using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace PlayerState {
    public class AirMove : BaseAirState, IAnimate
    {
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            _player.StateEvent.ExecuteEventOnce(EventType.OnMove, null);
            _player.IsMove = true;
        }

        public override void FixedUpdate()
        {
            if(_player.IsIdleFixed) return;
            
            base.FixedUpdate();
            _player.ActorMovement.CheckWall2();
            if(_player.ActorMovement.CheckMovable())
                _player.MoveComponent.ForceActorMovement.Move(_player.Direction, 1, false, _player.MoveResistFactor, _player.MaxMoveVel);
        }
        public override void OnExit()
        {
            _player.IsMove = false;
            base.OnExit();
        }

        public override void Update()
        {
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsMove, true);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsMove, false);
        }
    }
}