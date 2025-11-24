using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace PlayerState {
    public class Crouch : BaseGroundState, IAnimate
    {
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);

            _player.Stop();
            _player.Crouch();
        }
        public override void Update()
        {
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.StandUp();
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsCrouch, true);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.SetBool(EAnimationBool.IsCrouch, false);
        }
    }
}