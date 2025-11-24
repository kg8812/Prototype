using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerState;
using Unity.VisualScripting;

namespace PlayerState{
    public abstract class EventState : BaseState, IAutoEscape
    {
        public abstract EPlayerState NextState{ get; set; }

        public override void OnEnter(Player t)
        {
            base.OnEnter(t);
            _player.StateEvent.ExecuteEventOnce(EventType.OnEventState, null);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void Update()
        {
            // if(EscapeCondition()) _player.SetState(NextState);
        }

        public abstract bool EscapeCondition();
    }
}