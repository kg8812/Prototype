using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkeletonAttach : SkeletonAttach
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = Actor as Player;
    }

    public void ToStopState()
    {
        player?.SetState(EPlayerState.Stop);
    }

    public void SpawnIceDrill()
    {
        player?.EffectSpawner.Remove(Define.PlayerEffect.IceDrillLoop);
        player?.ActionController.SpawnIceDrill();
    }

    public void DrillComplete()
    {
        player?.StateEvent.ExecuteEventOnce(EventType.OnDrillComplete, null);
    }

    public void DrillExecute()
    {
        player?.SetState(EPlayerState.IceDrillExecute);
    }
}
