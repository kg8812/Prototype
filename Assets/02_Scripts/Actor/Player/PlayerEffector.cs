using Apis;
using Save.Schema;
using UnityEngine;
using System.Collections.Generic;
using Default;



public class PlayerEffector : EffectSpawner
{
    public enum CommonPlayerEffect{
    ONDASH, WALK, JUMP, REPAIR, HIT, STARTDASH, ENDDASH,
    }

    private readonly Player player;
    private FootPrintCreator footPrintCreator;


    public PlayerEffector(Player player) : base(player)
    {
        this.player = player;
        footPrintCreator = player.transform.GetComponentInParentAndChild<FootPrintCreator>();
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

    public void CreateFootPrint()
    {
        if (footPrintCreator != null)
        {
            footPrintCreator.SpawnFootPrint();
        }
    }
    
    private string GetEffectAddress(CommonPlayerEffect type)
    {
        var DashAddress = GetDashAddress();
        var WalkAddress = GetWalkAddress();
        string name = "";
        switch (type)
        {
            case CommonPlayerEffect.STARTDASH:
                name += DashAddress[0];
                break;
            case CommonPlayerEffect.ONDASH:
                name += DashAddress[1];
                break;
            case CommonPlayerEffect.ENDDASH:
                name += DashAddress[2];
                break;
            case CommonPlayerEffect.WALK:
                name += WalkAddress;
                break;
            case CommonPlayerEffect.REPAIR:
                name += Define.PlayerEffect.Repair;
                break;
            case CommonPlayerEffect.HIT:
                name += Define.PlayerEffect.Hit;
                break;
            default:
                break;
        }

        return name;
    }

    private string[] GetDashAddress()
    {
        return player.playerType switch {
                PlayerType.Gosegu => new string[] {Define.PlayerEffect.GoseguDashStart, Define.PlayerEffect.GoseguDashTrail, Define.PlayerEffect.GoesguDashEnd},
                PlayerType.Ine => new string[] {Define.PlayerEffect.IneDashStart, Define.PlayerEffect.IneDashTrail, Define.PlayerEffect.IneDashEnd},
                PlayerType.Jingburger => new string[] {Define.PlayerEffect.JingBurgerDashStart, Define.PlayerEffect.JingBurgerDashTrail, Define.PlayerEffect.JingBurgerDashEnd},
                PlayerType.Jururu => new string[] {Define.PlayerEffect.JururuDashStart, Define.PlayerEffect.JururuDashTrail, Define.PlayerEffect.JururuDashEnd},
                PlayerType.Lilpa => new string[] {Define.PlayerEffect.LilpaDashStart, Define.PlayerEffect.LilpaDashTrail, Define.PlayerEffect.LilpaDashEnd},
                PlayerType.Viichan => new string[] {Define.PlayerEffect.ViichanDashStart, Define.PlayerEffect.ViichanDashTrail, Define.PlayerEffect.ViichanDashEnd},
                _ => new string[] {"", "", ""}
        };
    }

    private string GetWalkAddress()
    {
        return player.playerType switch{
            PlayerType.Gosegu => Define.PlayerEffect.GoseguWalkLeaf,
            PlayerType.Ine => Define.PlayerEffect.IneWalkLeaf,
            PlayerType.Jingburger => Define.PlayerEffect.JingBurgerWalkLeaf,
            PlayerType.Jururu => Define.PlayerEffect.JururuWalkLeaf,
            PlayerType.Lilpa => Define.PlayerEffect.LilpaWalkLeaf,
            PlayerType.Viichan => Define.PlayerEffect.ViichanWalkLeaf,
            _ => ""
        };
    }
}
