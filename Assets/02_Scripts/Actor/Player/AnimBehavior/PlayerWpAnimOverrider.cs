using System;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Default;
using Save.Schema;
using UnityEngine;

#if  UNITY_EDITOR
#endif
public class PlayerWpAnimOverrider
{

    [HideInInspector] public int maxGroundAtk;
    [HideInInspector] public int maxAirAtk;

    private AnimatorOverrider overrider;
    
    static Dictionary<PlayerType, Dictionary<string, AnimationClip>> _clipDict;

    public static Dictionary<PlayerType, Dictionary<string, AnimationClip>> ClipDict
    {
        get
        {
            if (_clipDict == null)
            {
                _clipDict = new();
                foreach (PlayerType type in Enum.GetValues(typeof(PlayerType)))
                {
                    string playerName = type.ToString();

                    var clips = ResourceUtil.Load<RuntimeAnimatorController>($"{playerName}Controller").animationClips;
                    var dict = new Dictionary<string, AnimationClip>();
                    foreach (var a in clips)
                    {
                        dict.TryAdd(a.name, a);
                    }

                    _clipDict.Add(type, dict);
                }
            }

            return _clipDict;
        }
    }

    private static Dictionary<string, AnimationClip> _colliderDict;

    private static Dictionary<string, AnimationClip> ColliderDict
    {
        get
        {
            return _colliderDict ??= ResourceUtil.LoadAll<AnimationClip>("WeaponColliderAnim")
                .ToDictionary(kv => kv.name, kv => kv);
        }
    }
    
    private readonly List<string> groundMotions = new();
    private readonly List<string> airMotions = new();
    private readonly List<string> groundColliders = new();
    private readonly List<string> airColliders = new();
    private readonly List<string> groundLegMotions = new();
    private readonly List<string> airLegMotions = new();

    public PlayerWpAnimOverrider(int index,AnimatorOverrider _overrider)
    {
        overrider = _overrider;
        if (!WeaponData.DataLoad.TryGetMotionGroup(index, out var data)) return;

        groundMotions = new();
        airMotions = new();
        groundColliders = new();
        airColliders = new();
        groundLegMotions = new();
        airLegMotions = new();
        
        foreach (var t in data.groundMotions)
        {
            if (WeaponData.DataLoad.TryGetMotion(t, out var d))
            {
                groundMotions.Add(d.motionName);
            }
        }

        foreach (var t in data.airMotions)
        {
            if (WeaponData.DataLoad.TryGetMotion(t, out var d))
            {
                airMotions.Add(d.motionName);
            }
        }

       
        foreach (var t in data.groundColliders)
        {
            if (WeaponData.DataLoad.TryGetMotion(t, out var d))
            {
                groundColliders.Add(d.motionName);
            }
        }
        foreach (var t in data.airColliders)
        {
            if (WeaponData.DataLoad.TryGetMotion(t, out var d))
            {
                airColliders.Add(d.motionName);
            }
        }
        foreach (var t in data.groundLegMotions)
        {
            if(WeaponData.DataLoad.TryGetMotion(t, out var d))
            {
                groundLegMotions.Add(d.motionName);
            }
        }
        foreach (var t in data.airLegMotions)
        {
            if(WeaponData.DataLoad.TryGetMotion(t, out var d))
            {
                airLegMotions.Add(d.motionName);
            }
        }
    }
    [HideInInspector] public AnimatorOverrideController motions;
    
    public void SetWeaponAnimations(PlayerType type,Animator animator)
    {
        motions = AnimatorOverrider.PlayerOverriders[type];
        
        for (int i = 0; i < groundMotions.Count; i++)
        {
            motions[$"GroundAtk{i + 1}"] = ClipDict[type][$"battle/{groundMotions[i]}"];
        }
        
        for (int i = 0; i < airMotions.Count; i++)
        {
            motions[$"AirAtk{i + 1}"] = ClipDict[type][$"battle/{airMotions[i]}"];
        }
        for (int i = 0; i < groundColliders.Count; i++)
        {
            motions[$"GroundAtkCollider{i + 1}"] = ColliderDict[groundColliders[i]];
        }
        
        for (int i = 0; i < airColliders.Count; i++)
        {
            motions[$"AirAtkCollider{i + 1}"] = ColliderDict[airColliders[i]];
        }

        for(int i = 0; i < groundLegMotions.Count; i++)
        {
            motions[$"GroundAtkLeg{i + 1}"] = ClipDict[type][$"battle/{groundLegMotions[i]}"];
        }

        for(int i = 0; i < airLegMotions.Count; i++)
        {
            motions[$"AirAtkLeg{i + 1}"] = ClipDict[type][$"battle/{airLegMotions[i]}"];
        }
        
        maxGroundAtk = groundMotions.Count;
        maxAirAtk = airMotions.Count;
    }
}