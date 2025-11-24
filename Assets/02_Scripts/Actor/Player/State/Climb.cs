using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerState {
    public class Climb : EventState, IInterruptable, IAnimate
    {
        public override EPlayerState NextState { get => EPlayerState.Stop; set{} }

        public float InterruptTime { get => -1; set {} }
        public EPlayerState[] InteruptableStates { get => new[] { EPlayerState.Jump, EPlayerState.Dash }; set {} }
        private bool escapeFlag = false;
        private ClimbInfo info;
        private float refLength = 0;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            escapeFlag = false;

            _player.SetGravity(0);

            info = _player.GetInfo(EPlayerState.Climb) as ClimbInfo;

            _player.IsClimb = true;

            _player.StateEvent.AddEvent(EventType.OnIdleMotion, (e) => escapeFlag = true );

            refLength = 0 + _player.Collider.bounds.size.y;

            Physics2D.IgnoreCollision(_player.PlayerCollisionCollider, info.hit, true);

            _player.StateEvent.ExecuteEventOnce(EventType.OnClimbStart, null);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();

            Physics2D.IgnoreCollision(_player.PlayerCollisionCollider, info.hit, false);

            _player.ResetGravity();

            _player.IsClimb = false;

            _player.StopMoving();

            _player.RemoveInfo(EPlayerState.Climb);
        }

        public override bool EscapeCondition()
        {
            return escapeFlag;
        }

        public void OnEnterAnimate()
        {
            float ratio = GetRatio();
            Vector2 curRootMotionScale = _player.AnimController.GetRootMotionScale();
            switch (info.type)
            {
                case EClimbType.LEDGEHIGH:
                    _player.StateEvent.AddEvent(EventType.OnClimbMotionStart, (e) => _player.AnimController.SetRootMotionScale(1, curRootMotionScale.y));
                    _player.animator.Play("Climb", 0, ratio * 0.8f);
                    break;
                case EClimbType.LEDGELOW:
                    _player.StateEvent.AddEvent(EventType.OnClimbMotionStart, (e) => _player.AnimController.SetRootMotionScale(1, curRootMotionScale.y));
                    _player.animator.Play("ClimbLow", 0, ratio * 0.8f);
                    break;
                case EClimbType.PLATFORMHIGH:
                    _player.StateEvent.AddEvent(EventType.OnClimbMotionStart, (e) => _player.AnimController.SetRootMotionScale(0, curRootMotionScale.y));
                    _player.animator.Play("Climb", 0, ratio * 0.8f);
                    break;
                case EClimbType.PLATFORMLOW:
                    _player.StateEvent.AddEvent(EventType.OnClimbMotionStart, (e) => _player.AnimController.SetRootMotionScale(0, curRootMotionScale.y));
                    _player.animator.Play("ClimbLow", 0, ratio * 0.8f);
                    break;
                default:
                    break;
            }
        }

        public void OnExitAnimate()
        {
            _player.StateEvent.RemoveAllEvents(EventType.OnClimbMotionStart);
        }

        private float GetRatio()
        {
            if(info == null) return 0;

            Vector2 end = info.endPos;
            Vector2 start = _player.transform.position;
            float diff = end.y - start.y;

            if(info.type == EClimbType.LEDGELOW) refLength /= 2;
            return diff / refLength > 1 ? 0 : 1 - diff / refLength;
        }
    }
}
