using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine;

public class AnimatorOverrider : MonoBehaviour
{
    private static Dictionary<int, PlayerWpAnimOverrider> overriders;


    private static Dictionary<PlayerType, AnimatorOverrideController> _playerOverriders;

    private Animator animator;

    private Player player;

    public static Dictionary<PlayerType, AnimatorOverrideController> PlayerOverriders => _playerOverriders ??=
        new Dictionary<PlayerType, AnimatorOverrideController>
        {
            {
                PlayerType.Player1,
                Instantiate(ResourceUtil.Load<AnimatorOverrideController>(Define.PlayerAnimations.Overrider))
            }
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
            overriders = new Dictionary<int, PlayerWpAnimOverrider>();

            foreach (var idx in WeaponData.motionGroupDict.Keys)
                overriders.TryAdd(idx, new PlayerWpAnimOverrider(idx, this));
        }

        SetPlayerAnimations(player.playerType);
    }

    public void SetAnimation(int index, Player _player)
    {
        if (overriders.TryGetValue(index, out var overrider))
        {
            overrider.SetWeaponAnimations(_player.playerType, _player.animator);
            animator.SetInteger("MaxGroundAtk", overrider.maxGroundAtk);
            animator.SetInteger("MaxAirAtk", overrider.maxAirAtk);
        }
    }
}