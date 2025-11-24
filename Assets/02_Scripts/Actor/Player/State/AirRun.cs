
namespace PlayerState{
    public class AirRun : BaseAirState, IAnimate
    {
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            _player.IsRun = true;
            _player.IsMove = true;

            // 점프 시 달리기 계속 유지되게
            _player.StateEvent.AddEvent(EventType.OnJump, KeepRunning);

            _player.StateEvent.AddEvent(EventType.OnAnyState, (e) => {
                // 점프 제외 다른 state로 넘어갔을 때는 달리기 유지 이벤트 제거
                if(_player.CurrentState != EPlayerState.Jump)
                    _player.StateEvent.RemoveEvent(EventType.OnJump, KeepRunning);
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            _player.ActorMovement.CheckWall2();
            if(_player.ActorMovement.CheckMovable())
                _player.MoveComponent.ForceActorMovement.Move(_player.Direction, 1, false, 0.01f, _player.RunVel);
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.IsRun = false;
        }
        private void KeepRunning(EventParameters e) => _player.IsRun = true;
        public void OnEnterAnimate()
        {

        }

        public void OnExitAnimate()
        {

        }
    }
}
