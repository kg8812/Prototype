using System;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class Disappear : CommonActionNode
    {
        public float time;

        private Guid _guid;
        private float _startTime;

        public override void OnStart()
        {
            base.OnStart();
            _startTime = Time.time;
            _actor.Hide();
            _guid = _actor.AddInvincibility();
        }

        public override void OnSkip()
        {
            base.OnSkip();
            _actor.Appear();
            _actor.RemoveInvincibility(_guid);
        }

        public override State OnUpdate()
        {
            if (_startTime + time < Time.time)
            {
                _actor.Appear();
                _actor.RemoveInvincibility(_guid);
                return State.Success;
            }

            return State.Running;
        }
    }
}