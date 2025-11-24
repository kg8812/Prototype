using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace PlayerState {
    public class Dash : EventState, IInterruptable, IAnimate
    {
        public override EPlayerState NextState { get => EPlayerState.Stop; set{ } }
        public float InterruptTime { get => _player.DashDelayCancelTime; set {} }
        public EPlayerState[] InteruptableStates { get => new[] { EPlayerState.Move, EPlayerState.AirMove, EPlayerState.Attack, EPlayerState.Skill, EPlayerState.Crouch, EPlayerState.Dash, EPlayerState.Run }; set { } }
        private Player.IPlayerDash dashStrategy;
        private Tween dashTweener;
        private bool exitFlag;
        private Vector2 bufferSpeed;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            
            
            _player.ExecuteEvent(EventType.OnDash, new EventParameters(_player));
            _player.StateEvent.ExecuteEvent(EventType.OnDash, null);

            // y방향 속도 죽이기, 아닌 경우 대쉬 종료 후 위로 튀는 현상
            bufferSpeed = new Vector2(_player.Rb.velocity.x, 0);
            _player.Rb.velocity = bufferSpeed;

            dashStrategy = _player.DashStrategy;

            dashTweener = dashStrategy.Dash();

            _player.IsDash = true;

            exitFlag = false;

            dashTweener.onComplete += () => {
                dashTweener.Kill();
                // _NextState = EPlayerState.DashLanding;   // tween 끝까지 완료 시 DashLanding
            };
            dashTweener.onKill += () => {
                dashStrategy.DashEnd();
                exitFlag = true;
            };

            if(_player.onAir) _player.AirDashed++;

            _player.CoolDown.StartCd(EPlayerCd.Dash, _player.DashCoolTime);
            _player.CoolDown.StartCd(EPlayerCd.DashToAttack, _player.DashAttackCoolTime);
            _player.CoolDown.StartCd(EPlayerCd.DashToJump, _player.DashToJumpDelay);

            _player.StateEvent.AddEvent(EventType.OnLanding, (e) => _player.AirDashed = 0);
            _player.StateEvent.AddEvent(EventType.OnEventState, (e) => { if(_player.CurrentState != EPlayerState.DashLanding) _player.CoolDown.StopCd(EPlayerCd.Dash); });

            _player.Controller.SetCommandState(Command.ECommandType.Dash);
        }

        public override void OnExit()
        {
            base.OnExit();

            dashStrategy?.OnEnd();

            dashTweener?.Kill();
            
            _player.IsDash = false;

            _player.Rb.velocity = bufferSpeed;  // 대쉬 이전 속도 유지

            _player.CoolDown.StopCd(EPlayerCd.DashToAttack);
            _player.CoolDown.StopCd(EPlayerCd.DashToJump);

            _player.Controller.SetCommandState(Command.ECommandType.None);
        }

        public override bool EscapeCondition()
        {
            return exitFlag;
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.Dash);
            _player.AnimController.SetBool(EAnimationBool.IsDash, true);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsDash, false);
        }
    }


    public class DashLanding : EventState, IInterruptable, IAnimate
    {
        private bool exitFlag = false;
        private Tweener exitTweener;
        public override EPlayerState NextState { get => EPlayerState.Stop; set{} }

        public float InterruptTime { get => _player.DashDelayCancelTime; set {} }
        public EPlayerState[] InteruptableStates { get => new[] { EPlayerState.Move, EPlayerState.Attack, EPlayerState.Skill, EPlayerState.Crouch, EPlayerState.Dash, EPlayerState.Run }; set { } }

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            if(_player.onAir) 
            {
                // _player.SetState(EPlayerState.AirIdle);
                exitFlag = true;
                AddAbleState(EPlayerState.Run);
                return;
            }

            exitFlag = false;

            exitTweener = _player.DashLanding(_player.DashLandingTime, _player.DashDelayDistance, _player.DashDelaySpeedGraph);
            exitTweener.onComplete += () => exitFlag = true;

            _player.Controller.SetCommandState(Command.ECommandType.Dash);
        }

        public override bool EscapeCondition()
        {
            return exitFlag;
        }

        public override void OnExit()
        {
            base.OnExit();

            exitTweener?.Kill();

            _player.Controller.SetCommandState(Command.ECommandType.None);
        }

        public void OnEnterAnimate()
        {
        }

        public void OnExitAnimate()
        {
        }
    }
}