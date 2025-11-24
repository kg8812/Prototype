using UnityEngine;

namespace PlayerState{
    public class Run : BaseRunState, IAnimate
    {
        private Coroutine drillCoroutine;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            drillCoroutine = _player.StartTimer(_player.IceDrillEnterTime, () =>{
                AddAbleState(EPlayerState.IceDrillCharge);
            });
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.StopTimer(drillCoroutine);
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsRun, true);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsRun, false);
        }
    }
}
