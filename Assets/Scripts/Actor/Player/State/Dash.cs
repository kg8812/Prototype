using Command;
using DG.Tweening;
using UnityEngine;

namespace PlayerState
{
    public class Dash : EventState, IInterruptable, IAnimate
    {
        private Vector2 bufferSpeed;
        private IPlayerDash dashStrategy;
        private Tween dashTweener;
        private bool exitFlag;

        public override EPlayerState NextState
        {
            get => EPlayerState.Idle;
            set { }
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

        public float InterruptTime
        {
            get => _player.DashDelayCancelTime;
            set { }
        }

        public EPlayerState[] InterruptableStates => new[]
            { EPlayerState.Move, EPlayerState.Attack, EPlayerState.Skill, EPlayerState.Dash, EPlayerState.Run };

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            Debug.Log("Dash");

            _player.ExecuteEvent(EventType.OnDash, new EventParameters(_player));
            _player.StateEvent.ExecuteEvent(EventType.OnDash, null);

            // y방향 속도 죽이기, 아닌 경우 대쉬 종료 후 위로 튀는 현상
            bufferSpeed = new Vector2(_player.Rb.linearVelocity.x, 0);
            _player.Rb.linearVelocity = bufferSpeed;

            dashStrategy = _player.DashStrategy;

            dashTweener = dashStrategy.Dash();

            _player.IsDash = true;

            exitFlag = false;

            dashTweener.onComplete += () =>
            {
                dashTweener.Kill();
                // _NextState = EPlayerState.DashLanding;   // tween 끝까지 완료 시 DashLanding
            };
            dashTweener.onKill += () =>
            {
                dashStrategy.DashEnd();
                exitFlag = true;
            };

            if (_player.onAir) _player.AirDashed++;

            _player.CoolDown.StartCd(EPlayerCooldown.Dash, _player.DashCoolTime);
            _player.CoolDown.StartCd(EPlayerCooldown.DashToAttack, _player.DashAttackCoolTime);
            _player.CoolDown.StartCd(EPlayerCooldown.DashToJump, _player.DashToJumpDelay);

            _player.StateEvent.AddEvent(EventType.OnLanding, e => _player.AirDashed = 0);

            _player.Controller.SetCommandState(ECommandType.Dash);
        }

        public override void OnExit()
        {
            base.OnExit();

            Debug.Log("DashExit");
            dashStrategy?.OnEnd();

            dashTweener?.Kill();

            _player.IsDash = false;

            _player.Rb.linearVelocity = bufferSpeed; // 대쉬 이전 속도 유지

            _player.CoolDown.StopCd(EPlayerCooldown.DashToAttack);
            _player.CoolDown.StopCd(EPlayerCooldown.DashToJump);

            _player.Controller.SetCommandState(ECommandType.None);
        }

        public override bool EscapeCondition()
        {
            return exitFlag;
        }
    }
}