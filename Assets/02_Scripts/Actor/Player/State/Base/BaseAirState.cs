using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

namespace PlayerState{
    public class BaseAirState : BaseState
    {
    public override void OnEnter(Player t)
    {
        base.OnEnter(t);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        _player.MoveComponent.ForceActorMovement.Drop(_player.DropResistFactor, _player.MaxDropVel);
    }
    public override void OnExit()
    {
        base.OnExit();
    }

    public override void Update()
    {
    }
    }
}
