using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace PlayerState{
    public class BaseGroundState : BaseState
    {
        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            // _player.GravityOff();
        }
        public override void Update()
        {
        }
    }
}
