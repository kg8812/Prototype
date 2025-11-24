using System;
using System.Collections;
using System.Collections.Generic;
using EventData;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PlayerState {
    public class Interact : EventState, IAnimate
    {
        public override EPlayerState NextState { get => EPlayerState.Stop; set {} }
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void OnEnterAnimate()
        {
            // set interact anim set
            _player.AnimController.Trigger(EAnimationTrigger.Interact);
        }

        public void OnExitAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.InteractEnd);
        }

        public override bool EscapeCondition()
        {
            return true;
        }
    }
}
