using System.Collections.Generic;
using Apis;
using Apis;
using Default;
using NewNewInvenSpace;
using Sirenix.Utilities;
using Spine;
using Spine.Unity;
using Spine.Unity.Examples;
using Unity.VisualScripting;
using UnityEngine;

public partial class Player
{
    public struct WeaponAtkInfo
    {
        public int atkCombo;
        public AnimAtkMotionBehaviour.AirOrGround airOrGround;
    }
    // 플레이어 무기 관련

   public IPlayerAttack attackStrategy;

    [HideInInspector] public AnimatorOverrider _overrider;

    [HideInInspector] public WeaponAtkInfo weaponAtkInfo;

    public Transform orbPos;
    public Transform ineBookPos;
    Bone weaponBone;
    public Bone WeaponBone => weaponBone ??= Mecanim.skeleton.FindBone("weapon");

    private WeaponSpriteAttacher _attacher;

    public WeaponSpriteAttacher Attacher
    {
        get
        {
            return _attacher ??= transform.GetComponentInParentAndChild<WeaponSpriteAttacher>();
        }
    }

    private AtlasRegionAttacher _regionAttacher;

    public AtlasRegionAttacher RegionAttacher =>
        _regionAttacher ??= transform.GetComponentInParentAndChild<AtlasRegionAttacher>();

    public bool BlockSkillChange = false;

    public void SetWeaponSpriteAttacher(Weapon weapon)
    {
        if (weapon == null) return;
        Attacher.RemoveAll();
        RegionAttacher.RemoveAll();
        
        if (!weapon.isSpine)
        {
            Attacher.sprites = weapon.sprites;
            Attacher.materials = weapon.materials;
            Attacher.slots = weapon.slots;
            
            Attacher.Initialize();
            Attacher.Attach();
        }
        else
        {
            RegionAttacher.atlasAsset = weapon.atlasAsset;
            for (int i = 0; i < weapon.slots.Length; i++)
            {
                AtlasRegionAttacher.SlotRegionPair pair = new();
                pair.slot = weapon.slots[i];
                pair.region = weapon.atlasRegion[i];
                _regionAttacher.attachments.Add(pair);
            }
            RegionAttacher.Apply(Mecanim);
        }
    }
    
    // 무조건 무기 공격을 하는것에서 AtkStrategy로 변경했음.
    // 비챤 야수모드처럼 다른 콤보공격을 해야하는 경우도 생겨서 필요한 변경사항
    
    public void Attack(int _)
    {
        attackStrategy.Attack(weaponAtkInfo.atkCombo);
    }

    // 무조건 무기 공격을 하는것에서 AtkStrategy로 변경했음.
    // 비챤 야수모드처럼 다른 콤보공격을 해야하는 경우도 생겨서 필요한 변경사항
    public void AttackInCombo(int combo)
    {
        attackStrategy.Attack(combo);
    }
    public void SetAttack(IPlayerAttack attack)
    {
        attackStrategy = attack;
    }

    public void SetAttackToNormal()
    {
        attackStrategy = new PlayerWeaponAttack(this);
    }

    public void Attack()
    {
        var item = AttackItemManager.CurrentItem;
        
        if (item != null && attackStrategy.CheckAttackable(item.AtkSlotIndex))
        {
            attackStrategy.Attack();
        }
    }
    
    public void DoWeaponSkillAction(int index) 
    {
        if (AttackItemManager.CurrentItem is ActiveSkillItem { ActiveSkill: { } skill })
        {
            if (skill.actionList.Count > index)
            {
                skill.actionList[index].Invoke();
            }
        }
    }
    public void AfterWeaponAtk()
    {
        if (AttackItemManager.CurrentItem is Weapon weapon)
        {
            weapon.AfterAtk();
        }
    }

    public void OnAttackItemChange()
    {
        weaponAtkInfo.atkCombo = 0;
        TurnOffBoneFollower();
    }
    public void TurnOffBoneFollower(bool disAppear = true) // 애니메이션에서 사용
    {
        if (AttackItemManager.CurrentItem is Weapon { IsFollow: false } weapon)
        {
            weapon.BoneFollower?.ForEach(x => x.enabled = false);
            
            if (disAppear && weapon.appearUse)
            {
                weapon.wpSprites.ForEach(x =>
                {
                    x.Disappear();
                });
            }
        }
    }

    public void TurnOnBoneFollower(bool appear = true) // 애니메이션에서 사용
    {
        if (AttackItemManager.CurrentItem is Weapon { IsFollow: false } weapon)
        {
            weapon.BoneFollower?.ForEach(x => x.enabled = true);
            if (appear && weapon.appearUse)
            {
                weapon.wpSprites.ForEach(x => { x.Appear(); });
            }
        }
    }

    public void Slash()
    {
        EventParameters param = new(this);
        ExecuteEvent(EventType.OnWeaponSlash, param);
    }

    public void ApplyPlayerPreset()
    {
        InvenManager.instance.PresetManager.ApplyPreset((int)playerType);
    }
}