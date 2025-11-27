using Apis;
using UnityEngine;

public class PlayerEffector : EffectSpawner
{
    public enum CommonPlayerEffect
    {
    }

    private readonly Player player;


    public PlayerEffector(Player player) : base(player)
    {
        this.player = player;
    }

    public ParticleDestroyer GetParticle(CommonPlayerEffect type, bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), player.Position, false, disappearWhenHide);
    }

    public ParticleDestroyer GetParticle(CommonPlayerEffect type, Vector2 position, bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), position, false, disappearWhenHide);
    }

    public ParticleDestroyer Play(CommonPlayerEffect type, Vector2 position, bool isFollow, bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), position, isFollow, disappearWhenHide);
    }

    public void Stop(ParticleDestroyer effect)
    {
        if (effect == null) return;

        Remove(effect);
    }

    private string GetEffectAddress(CommonPlayerEffect type)
    {
        var name = "";

        return name;
    }
}