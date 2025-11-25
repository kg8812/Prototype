using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using PlayerState;
using UnityEngine;

namespace PlayerState {
    public class Move : BaseGroundState, IAnimate
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
            else
                _player.StopMoving();

            float _moveMultiplier = 1 * _player.MoveSpeed / _player.StatManager.BaseStat.Get(ActorStatType.MoveSpeed);
            float normalizedSpeed = _player.Rb.linearVelocity.magnitude / _player.MaxMoveVel * Mathf.Sign(_player.Rb.linearVelocity.x);
            // Threshold 1을 기준으로 정규화
            _player.AnimController.SetFloat(EAnimationFloat.MoveSpeed, normalizedSpeed * _moveMultiplier);
        }
        public override void OnExit()
        {
            _player.IsMove = false;
            base.OnExit();
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