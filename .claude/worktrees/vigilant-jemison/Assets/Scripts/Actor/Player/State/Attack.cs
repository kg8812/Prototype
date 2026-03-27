using Command;
using UnityEngine;

namespace PlayerState
{
    public class Attack : EventState, IAnimate
    {
        private bool escapeFlag;

        public override EPlayerState NextState
        {
            get => EPlayerState.Idle;
            set { }
        }

        public void OnEnterAnimate()
        {
            if (_player.PressingDir != 0)
                _player.AnimController.ActivateLeg();
            else
                _player.AnimController.DeactivateLeg();

            
            _player.AnimController.ResetTrigger(EAnimationTrigger.Attack);
            _player.AnimController.Trigger(EAnimationTrigger.AttackInit);
            
        }

        public void OnExitAnimate()
        {
        }

        public override void OnEnter(Player t)
        {
            escapeFlag = false;

            base.OnEnter(t);

            _player.OnAttack = true;

            _player.SetGravity(0);

            _player.Stop();

            // TODO: AttackEvent 작업 완료 후 삭제
            // if(_player.PressingDir != 0)
            //     _player.Step(_player.Direction);

            _player.ExecuteEvent(EventType.OnAttackStateEnter, null);

            // 공격 넣기 player.Attack()
            
            _player.StateEvent.ExecuteEventOnce(EventType.OnAttackStateEnter, null);

            _player.StateEvent.AddEvent(EventType.OnAttackSuccess, Escape);

            _player.StateEvent.AddEvent(EventType.OnIdleMotion, Cancel);

            _player.StateEvent.AddEvent(EventType.OnEventState, Cancel);

            _player.Controller.SetCommandState(ECommandType.Attack);
        }

        public override void OnExit()
        {
            base.OnExit();

            _player.StateEvent.RemoveEvent(EventType.OnAttackSuccess, Escape);

            _player.StateEvent.RemoveEvent(EventType.OnIdleMotion, Cancel);

            _player.StateEvent.RemoveEvent(EventType.OnEventState, Cancel);

            _player.OnAttack = false;

            _player.Controller.SetCommandState(ECommandType.None);
        }

        public override bool EscapeCondition()
        {
            return escapeFlag;
        }

        private void Escape(EventParameters e)
        {
            escapeFlag = true;
        }

        private void Cancel(EventParameters e)
        {
            var currentState = _player.GetState();
            Debug.Log("Cancel");

            _player.SetState(EPlayerState.Idle);

            _player.ResetGravity();
        }
    }
}