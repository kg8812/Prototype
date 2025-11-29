namespace PlayerState
{
    public class Idle : BaseGroundState, IAnimate
    {
        public void OnEnterAnimate()
        {
        }

        public void OnExitAnimate()
        {
        }

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            _player.StateEvent.ExecuteEventOnce(EventType.OnIdle, null);
        }

        public override void FixedUpdate()
        {
            if (_player.IsIdleFixed) return;

            base.FixedUpdate();

            _player.MoveComponent.ForceActorMovement.Friction(5);
            _player.resister.Resist();
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}