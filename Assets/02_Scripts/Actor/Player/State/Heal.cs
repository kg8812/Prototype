using DG.Tweening;
using UnityEngine;

namespace PlayerState {
    public class Heal : EventState, IAnimate, IInterruptable
    {
        public override EPlayerState NextState { get => EPlayerState.HealEnd; set {} }

        public float InterruptTime { get => 0; set{} }
        public EPlayerState[] InteruptableStates { get => new[] {EPlayerState.Dash, EPlayerState.Jump } ; set {} }

        Sequence seq;
        private bool escapeFlag;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            seq = DOTween.Sequence().SetAutoKill(true);
            escapeFlag = false;
            seq.SetDelay(_player.PotionUseTime).OnKill(()=>{
                escapeFlag = true;
            });
            seq.SetUpdate(UpdateType.Fixed);
            _player.StopMoving();
        }

        public override void OnExit()
        {
            base.OnExit();

            seq?.Kill();
        }

        public override bool EscapeCondition()
        {
            return escapeFlag;
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.SetFloat(EAnimationFloat.PotionTime, 1/_player.PotionUseTime);
            _player.AnimController.Trigger(EAnimationTrigger.Heal);
        }

        public void OnExitAnimate()
        {
        }
    }

    public class HealEnd : EventState, IInterruptable
    {
        public override EPlayerState NextState { get => EPlayerState.Idle; set{} }
        private bool healed;
        public float InterruptTime { get => 0; set{} }
        public EPlayerState[] InteruptableStates { get => new[] { EPlayerState.Dash, EPlayerState.Jump }; set{} }
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            healed = false;
            _player.StateEvent.AddEvent(EventType.OnRepair, (e)=> healed = true );
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.StateEvent.RemoveAllEvents(EventType.OnRepair);
        }

        public override bool EscapeCondition()
        {
            return healed;
        }
    }
}