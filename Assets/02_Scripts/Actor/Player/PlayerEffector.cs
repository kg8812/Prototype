using Apis;
using Save.Schema;
using UnityEngine;
using System.Collections.Generic;
using Default;



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

    public ParticleDestroyer GetParticle(CommonPlayerEffect type,bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), player.Position, false,disappearWhenHide);
    }

    public ParticleDestroyer GetParticle(CommonPlayerEffect type, Vector2 position,bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), position, false,disappearWhenHide);
    }

    public ParticleDestroyer GetParticleWithSpine(CommonPlayerEffect type,bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), "center", false,disappearWhenHide);
    }
    public ParticleDestroyer Play(CommonPlayerEffect type, bool isFollow ,bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), "center", isFollow, disappearWhenHide);
        
    }

    public ParticleDestroyer Play(CommonPlayerEffect type, Vector2 position,  bool isFollow ,bool disappearWhenHide)
    {
        return Spawn(GetEffectAddress(type), position, isFollow, disappearWhenHide);
        
    }

    public void Stop(ParticleDestroyer effect)
    {
        if(effect == null) return;

        Remove(effect);
    }
    
    private string GetEffectAddress(CommonPlayerEffect type)
    {
        string name = "";

        return name;
    }
}
