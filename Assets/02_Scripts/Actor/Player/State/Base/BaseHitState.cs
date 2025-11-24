using System.Collections;
using System.Collections.Generic;
using EventData;
using PlayerState;
using Unity.VisualScripting;
using UnityEngine;

namespace PlayerState {
    public class BaseHitState : EventState
    {
        public override EPlayerState NextState { get{ return EPlayerState.Idle; } set{} }
        protected bool escapeFlag;

        protected EventParameters eventParameters;
        protected KnockBackData data;

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            escapeFlag = false;
            // _player.StopHitEffect();
            // _player.PlayHitEffect();
            _player.StateEvent.ExecuteEventOnce(EventType.OnHit, null);
            var e = _player.GetInfo(EPlayerState.KnockBack);
            eventParameters = e.eventParameters;
            data = _player.GetKnockBackData(eventParameters);
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.RemoveInfo(EPlayerState.KnockBack);
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
            {
                _player.MoveComponent.KnockBack(knockBackSrc, _player.knockBackData,
                    null, null);
            }
            else
            {
                _player.MoveComponent.KnockBack(knockBackSrc, data, null, null);
            }
        }
    }
}
