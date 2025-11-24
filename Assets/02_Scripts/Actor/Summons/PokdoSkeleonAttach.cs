using Default;
using UnityEngine;

public class PokdoSkeleonAttach : SkeletonAttach
{
    public enum EffectType
    {
        JururuSoulKnightFireball,
        JururuSoulKnightRelease,
        JururuSoulKnightScratch,
        JururuSoulKnightScratchFin,
        JururuSoulKnightSummon,
        JururuSoulKnightTeleport,
    }

    private PokdoStand pokdo;

    string GetEffectAddress(EffectType type)
    {
        string address = type switch
        {
            EffectType.JururuSoulKnightFireball => Define.PlayerEffect.JururuSoulKnightFireball,
            EffectType.JururuSoulKnightRelease => Define.PlayerEffect.JururuSoulKnightRelease,
            EffectType.JururuSoulKnightScratch => Define.PlayerEffect.JururuSoulKnightScratch,
            EffectType.JururuSoulKnightScratchFin => Define.PlayerEffect.JururuSoulKnightScratchFin,
            EffectType.JururuSoulKnightSummon => Define.PlayerEffect.JururuSoulKnightSummon,
            EffectType.JururuSoulKnightTeleport => Define.PlayerEffect.JururuSoulKnightTeleport,
            _ => ""
        };

        return address;
    }
    public void SpawnVFX(EffectType type)
    {
        string address = GetEffectAddress(type);
        SpawnVFX(address);
    }

    public void SpawnVFXInSpine(EffectType type)
    {
        SpawnVFXInSpine(GetEffectAddress(type));
    }
    protected override void Awake()
    {
        pokdo = Utils.GetComponentInParentAndChild<PokdoStand>(gameObject);
    }

    public void PokdoAttack(int combo)
    {
        pokdo.Attack(combo);
    }

    public void Pull(float time)
    {
        pokdo.Pull(time);
    }
}