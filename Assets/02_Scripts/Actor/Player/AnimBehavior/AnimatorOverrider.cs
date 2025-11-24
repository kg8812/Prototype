using System;
using System.Collections.Generic;
using Apis;
using Default;
using Save.Schema;
using UnityEngine;
using UnityEngine.AddressableAssets;
public class AnimatorOverrider : MonoBehaviour
{
    private static Dictionary<int, PlayerWpAnimOverrider> overriders;
    
    private Animator animator;

    private Player player;


    private static Dictionary<PlayerType, AnimatorOverrideController> _playerOverriders;

    public static Dictionary<PlayerType, AnimatorOverrideController> PlayerOverriders => _playerOverriders ??= new()
    {
        { PlayerType.Ine ,Instantiate(ResourceUtil.Load<AnimatorOverrideController>(Define.Animations.IneOverrider))},
        { PlayerType.Jingburger ,Instantiate(ResourceUtil.Load<AnimatorOverrideController>(Define.Animations.JingburgerOverrider))},
        { PlayerType.Lilpa ,Instantiate(ResourceUtil.Load<AnimatorOverrideController>(Define.Animations.LilpaOverrider))},
        { PlayerType.Jururu ,Instantiate(ResourceUtil.Load<AnimatorOverrideController>(Define.Animations.JururuOverrider))},
        { PlayerType.Gosegu ,Instantiate(ResourceUtil.Load<AnimatorOverrideController>(Define.Animations.GoseguOverrider))},
        { PlayerType.Viichan ,Instantiate(ResourceUtil.Load<AnimatorOverrideController>(Define.Animations.ViichanOverrider))},
    };
    public void SetPlayerAnimations(PlayerType type)
    {
        animator.runtimeAnimatorController = PlayerOverriders[type];
    }
    public void Init(Player _player)
    {
        animator = GetComponent<Animator>();
        player = _player;
        if (overriders == null)
        {
            overriders = new();

            foreach (var idx in WeaponData.motionGroupDict.Keys)
            {
                overriders.TryAdd(idx, new PlayerWpAnimOverrider(idx,this));
            }
        }
        SetPlayerAnimations(player.playerType);
    }
    public void SetAnimation(int index,Player _player)
    {
        if (overriders.TryGetValue(index, out var overrider))
        {
            overrider.SetWeaponAnimations(_player.playerType,_player.animator);
            animator.SetInteger("MaxGroundAtk",overrider.maxGroundAtk);
            animator.SetInteger("MaxAirAtk",overrider.maxAirAtk);
        }
    }
}
