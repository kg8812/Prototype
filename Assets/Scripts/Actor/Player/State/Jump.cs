namespace PlayerState
{
    public class Jump : EventState, IAnimate
    {
        public override EPlayerState NextState
        {
            get => EPlayerState.Idle;
            set { }
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.Jump);
        }

        public void OnExitAnimate()
        {
        }

        public override bool EscapeCondition()
        {
            return _player.onAir;
        }

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            _player.StateEvent.ExecuteEventOnce(EventType.OnJump, null);

            _player.ExecuteEvent(EventType.OnJump, new EventParameters(_player));

            _player.MoveComponent.ForceActorMovement.Jump(_player.JumpForce);

            if (_player.CoyoteCurrentJump == 0) _player.CoyoteCurrentJump.Value = 1;

            else if (_player.CoyoteCurrentJump > 0) _player.CoyoteCurrentJump.Value++;

            _player.CoolDown.StartCd(EPlayerCd.JumpToAttack, _player.JumpAttackCoolTime);

            _player.StateEvent.AddEvent(EventType.OnEventState, e => _player.CoolDown.StopCd(EPlayerCd.JumpToAttack));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}