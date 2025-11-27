using System;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

public class ProjectileWeapon : Weapon
{
    private IWeaponAttack iAttack;

    [TitleGroup("공격설정")] [TabGroup("공격설정/설정", "지상")] [ShowIf("attackType", AttackType.Projectile)] [LabelText("투사체 설정")]
    public List<ProjInfo> groundProjectileInfos;

    [TabGroup("공격설정/설정", "공중")] [ShowIf("attackType", AttackType.Projectile)] [LabelText("투사체 설정")]
    public List<ProjInfo> airProjectileInfos;

    private List<float> groundCancel;
    private List<float> airCancel;
    public override AttackType attackType => AttackType.Projectile;

    public override IWeaponAttack IAttack
    {
        get
        {
            iAttack ??= new ProjectileAttack(this);
            return iAttack;
        }
    }

    public override List<float> GroundCancelTimes =>
        groundCancel ??= groundProjectileInfos.Select(x => x.cancelTime).ToList();

    public override List<float> AirCancelTimes =>
        airCancel ??= groundProjectileInfos.Select(x => x.cancelTime).ToList();

    [Serializable]
    public struct ProjInfo
    {
        [LabelText("투사체 설정")] public ProjectileInfo info;
        [LabelText("발사 개수")] public int projCount;

        [LabelText("발사 시간간격")] [Tooltip("여러개 발사할 시, 발사 사이의 시간간격")]
        public float fireTerm;

        [LabelText("그로기 계수(%)")] public float groggy;
        [LabelText("캔슬 시간 계수")] public float cancelTime;
        [LabelText("투사체 타입")] public ProjectileAttack.ProjectileType projType;
    }
}