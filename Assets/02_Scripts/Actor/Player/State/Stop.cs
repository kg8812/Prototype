using System.Collections;
using System.Collections.Generic;
using Apis;
using PlayerState;
using UnityEngine;

namespace PlayerState {
    public class Stop : BaseState, IAnimate
    {
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            _player.StateEvent.ExecuteEventOnce(EventType.OnStop, null);

            var next = _player.onAir ? EPlayerState.AirIdle : EPlayerState.Idle;

            _player.SetState(next);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }
        public override void OnExit()
        {
            base.OnExit();
            if(!_player.onAir)
                _player.StopMoving(); // 지면에서는 y방향 속도 필요없다고 판단(아니면 경사 오르다 멈출 시 계속 위로 오르려 함)
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.Stop);
        }

        public void OnExitAnimate()
        {
        }
    }
}