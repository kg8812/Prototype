using System.Collections;
using System.Collections.Generic;
using PlayerState;
using UnityEngine;

public class Test : EventState
{
    public override EPlayerState NextState { get => EPlayerState.Stop; set {} }

    public override bool EscapeCondition()
    {
        return false;
    }

    public override void OnEnter(Player t)
    {
        base.OnEnter(t);

        _player.ForceDash(10, EDirection.Up & EDirection.Right);
    }
}
