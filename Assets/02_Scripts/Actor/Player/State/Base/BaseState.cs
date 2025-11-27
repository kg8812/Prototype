using System.Collections.Generic;

namespace PlayerState
{
    public abstract class BaseState : IState<Player>
    {
        // protected EPlayerState[] AbleStates;
        private List<EPlayerState> _AbleStates;
        protected Player _player;

        public virtual List<EPlayerState> AbleStates
        {
            get
            {
                _AbleStates ??= new List<EPlayerState>();
                return _AbleStates;
            }
            set => _AbleStates = value;
        }

        public virtual void OnEnter(Player t)
        {
            _player = t;

            AbleStates.Clear();

            var states = NextState.Get(_player.CurrentState);
            if (states != null) AbleStates.AddRange(states);

            foreach (var state in AbleStates)
                _player.SetAbleState(state);

            _player.StateEvent.ExecuteEventOnce(EventType.OnAnyState, null);
        }

        public virtual void FixedUpdate()
        {
            var isStick = _player.ActorMovement.IsStick;
            if (_player.onAir)
            {
                if (!_player.IsFixGravity) _player.GravityOn();
                if (isStick)
                {
                    /* 착지 */
                    _player.StateEvent.ExecuteEventOnce(EventType.OnLanding, null);
                    _player.onAir = false;
                    _player.CoyoteCurrentJump.Value = 0;
                    _player.AnimController.SetBool(EAnimationBool.OnAir, false);
                }
            }
            else
            {
                if (!_player.IsFixGravity) _player.GravityOff();
                if (!isStick)
                {
                    /* 땅 -> 공중 */
                    _player.StateEvent.ExecuteEventOnce(EventType.OnAirEnter, null);
                    _player.onAir = true;
                    _player.CoyoteCurrentJump.CoyoteSet(1);
                    _player.AnimController.SetBool(EAnimationBool.OnAir, true);
                }
            }
        }

        public virtual void OnExit()
        {
            foreach (var state in AbleStates)
                _player.SetAbleState(state, false);
            _player.SetAbleState(_player.CurrentState, false);
        }

        public virtual void Update()
        {
        }

        public void AddAbleState(EPlayerState state)
        {
            AbleStates.Add(state);
            _player.SetAbleState(state);
        }

        public void RemoveAbleState(EPlayerState state)
        {
            AbleStates.Remove(state);
            _player.SetAbleState(state, false);
        }
    }
}