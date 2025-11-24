using System;
using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace PlayerState {
    public class Dead : BaseState, IAnimate
    {
        private Guid guid;
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            guid = _player.AddInvincibility();
            GameManager.PlayerController = null;
            _player.StopMoving();
            GameManager.instance.playerDied = true;
            _player.IsDead = true;
        }

        public void OnEnterAnimate()
        {
            _player.AnimController.Trigger(EAnimationTrigger.Dead);
        }

        public override void OnExit()
        {
            base.OnExit();
            _player.IsDead = false;
            _player.RemoveInvincibility(guid);
            GameManager.PlayerController = _player.GetComponent<ActorController>();
            GameManager.instance.playerDied = false;
        }

        public void OnExitAnimate()
        {
        }
    }
}