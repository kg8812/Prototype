using Apis;
using Sirenix.Utilities;
using Spine;
using Spine.Unity;
using UnityEngine;

public partial class Player : IMecanimUser
{
    [HideInInspector] public AnimatorOverrider _overrider;

    public Transform orbPos;
    public Transform ineBookPos;

    private SpineRenderer _spineRender;
    // 플레이어 무기 관련

    public IPlayerAttack attackStrategy;

    [HideInInspector] public WeaponAtkInfo weaponAtkInfo;

    private Bone weaponBone;
    private SpineRenderer spineRender => _spineRender ??= ActorRenderer as SpineRenderer;

    public Bone WeaponBone
    {
        get { return weaponBone ??= (actorRenderer as SpineRenderer)?.Mecanim.skeleton.FindBone("weapon"); }
    }

    public Transform SkeletonTrans
    {
        get => spineRender.SkeletonTrans;
        set => spineRender.SkeletonTrans = value;
    }

    public SkeletonMecanim Mecanim
    {
        get => spineRender.Mecanim;
        set => spineRender.Mecanim = value;
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

        if (item != null && attackStrategy.CheckAttackable(item.AtkSlotIndex)) attackStrategy.Attack();
    }

    public void DoWeaponSkillAction(int index)
    {
        if (AttackItemManager.CurrentItem is ActiveSkillItem { ActiveSkill: { } skill })
            if (skill.actionList.Count > index)
                skill.actionList[index].Invoke();
    }

    public void AfterWeaponAtk()
    {
        if (AttackItemManager.CurrentItem is Weapon weapon) weapon.AfterAtk();
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
                weapon.wpSprites.ForEach(x => { x.Disappear(); });
        }
    }

    public void TurnOnBoneFollower(bool appear = true) // 애니메이션에서 사용
    {
        if (AttackItemManager.CurrentItem is Weapon { IsFollow: false } weapon)
        {
            weapon.BoneFollower?.ForEach(x => x.enabled = true);
            if (appear && weapon.appearUse) weapon.wpSprites.ForEach(x => { x.Appear(); });
        }
    }

    public void Slash()
    {
        EventParameters param = new(this);
        ExecuteEvent(EventType.OnWeaponSlash, param);
    }

    public struct WeaponAtkInfo
    {
        public int atkCombo;
        public AnimAtkMotionBehaviour.AirOrGround airOrGround;
    }
}