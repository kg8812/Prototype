using Apis;

namespace PlayerState
{
    public class Skill : EventState, IInterruptable
    {
        private EPlayerState[] _interuptableStates;

        private bool escape;

        //TODO: OnFinalAttack 임시 작업
        public override EPlayerState NextState
        {
            get => EPlayerState.Idle;
            set { }
        }

        public float InterruptTime { get; set; } = 0;

        public EPlayerState[] InteruptableStates =>
            _interuptableStates ??= new[]
            {
                EPlayerState.Dash
            };

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            _player.IsSkill = true;
            _player.Stop();

            if (_player.onAir)
                _player.OnFinalSkill = true;

            escape = false;

            _player.curSkill?.OnAfterDuration.AddListener(Escape);

            _player.StateEvent.AddEvent(EventType.OnIdleMotion, Escape);

            _player.StateEvent.AddEvent(EventType.OnSkillEnd, Escape);
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.OnFinalAttack = false;
            _player.IsSkill = false;
            _player.curSkill?.EndMotion();
            _player.curSkill?.OnAfterDuration.RemoveListener(Escape);
            _player.StateEvent.RemoveEvent(EventType.OnIdleMotion, Escape);
            _player.StateEvent.RemoveEvent(EventType.OnSkillEnd, Escape);
        }

        public override bool EscapeCondition()
        {
            return escape;
        }

        private void Escape()
        {
            escape = true;
        }

        private void Escape(EventParameters e)
        {
            Escape();
        }
    }

    public class Charging : EventState, IInterruptable
    {
        private EPlayerState[] _interuptableStates;
        private bool escape;

        private ActiveSkill skill;

        public override EPlayerState NextState
        {
            get => EPlayerState.Idle;
            set { }
        }

        public float InterruptTime { get; set; } = 0;

        public EPlayerState[] InteruptableStates =>
            _interuptableStates ??= new[]
            {
                EPlayerState.Dash
            };

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            _player.IsSkill = true;

            escape = false;
            _player.Stop();
            skill = _player.curSkill;
            _player.OnChargeStart.Invoke();
            _player.StateEvent.AddEvent(EventType.OnIdleMotion, e => escape = true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (_player.PressingDir != 0)
            {
                _player.ActorMovement.Move(_player.Direction, (100 - skill.chargeMoveDebuff) / 100f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.Stop();
            _player.IsSkill = false;
            _player.OnFinalAttack = false;
            _player.OnChargeEnd.Invoke();
        }

        public override bool EscapeCondition()
        {
            return escape;
        }
    }

    public class Casting : EventState, IInterruptable
    {
        private EPlayerState[] _interuptableStates;

        public override EPlayerState NextState
        {
            get => EPlayerState.Idle;
            set { }
        }

        public float InterruptTime { get; set; } = 0;

        public EPlayerState[] InteruptableStates =>
            _interuptableStates ??= new[]
            {
                EPlayerState.Dash,
                EPlayerState.Jump
            };

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            _player.Stop();
            _player.IsSkill = true;
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.IsSkill = false;
            _player.OnFinalAttack = false;
        }

        public override bool EscapeCondition()
        {
            return false;
        }
    }
}