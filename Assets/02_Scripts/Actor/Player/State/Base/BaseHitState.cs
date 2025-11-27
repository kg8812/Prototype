using EventData;
using UnityEngine;

namespace PlayerState
{
    public class BaseHitState : EventState
    {
        protected KnockBackData data;
        protected bool escapeFlag;

        protected EventParameters eventParameters;

        public override EPlayerState NextState
        {
            get => EPlayerState.Idle;
            set { }
        }

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            escapeFlag = false;
            // _player.StopHitEffect();
            // _player.PlayHitEffect();
            _player.StateEvent.ExecuteEventOnce(EventType.OnHit, null);
            data = _player.GetKnockBackData(eventParameters);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override bool EscapeCondition()
        {
            return escapeFlag;
        }

        protected void PhysicalEvent(EventParameters eventParameters, KnockBackData data)
        {
            // direction type에 따른 넉백 적용 vector2 계산
            Vector2 knockBackSrc = data.directionType == KnockBackData.DirectionType.AktObjRelative
                ? eventParameters.user.Position
                : eventParameters.master.Position;

            if (data.knockBackForce == 0)
                _player.MoveComponent.KnockBack(knockBackSrc, _player.knockBackData,
                    null, null);
            else
                _player.MoveComponent.KnockBack(knockBackSrc, data, null, null);
        }
    }
}