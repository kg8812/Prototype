using Apis;
using Apis;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Apis
{
    [CreateAssetMenu(fileName = "ProjectileInfo", menuName = "Scriptable/Datas/ProjectileInfo")]
    public class ProjectileInfo : ScriptableObject
    {
        [TabGroup("공격 오브젝트 설정")] [LabelText("피격 반응 여부")]
        public bool isHitReaction;
        
        [TabGroup("공격 오브젝트 설정")] [LabelText("피격 반응 유형")]
        public AttackEventData.HitReactionType hitReactionType;

        [TabGroup("공격 오브젝트 설정")] [LabelText("일반 넉백 = 그로기 넉백")]
        public bool isSameDefaultGroggyKnockBack;

        [TabGroup("공격 오브젝트 설정")] [LabelText("일반 넉백 데이터")]
        public KnockBackData knockBackData;
        
        [HideIf("isSameDefaultGroggyKnockBack", false)]
        [TabGroup("공격 오브젝트 설정")] [LabelText("그로기 넉백 데이터")]
        public KnockBackData groggyKnockBackData;

        [TabGroup("공격 오브젝트 설정")] [LabelText("타겟 레이어")]
        public LayerMask targetLayer;

        [FormerlySerializedAs("baseAddAtkCount")] [TabGroup("공격 오브젝트 설정")] [LabelText("공격여부")]
        public bool isAtk = true;

        [TabGroup("공격 오브젝트 설정")] [LabelText("추가공격 횟수 (다단히트시)")]
        public int AdditionalAtkCount;

        [TabGroup("공격 오브젝트 설정")] [LabelText("데미지")]
        public float dmg;

        [TabGroup("공격 오브젝트 설정")] [LabelText("지속시간")]
        public float duration;

        [TabGroup("공격 오브젝트 설정")] [LabelText("공격 판정")]
        public Define.AttackType attackType = Define.AttackType.BasicAttack;

        [TabGroup("공격 오브젝트 설정")] [LabelText("공격타입")]
        public AttackObject.AttackTypeEnum atkTypeEnum;

        [ShowIf("atkTypeEnum", AttackObject.AttackTypeEnum.Tick)] [TabGroup("공격 오브젝트 설정")] [LabelText("공격주기")]
        public float frequency;

        [ShowIf("atkTypeEnum", AttackObject.AttackTypeEnum.Tick)]
        [TabGroup("공격 오브젝트 설정")]
        [LabelText("첫 공격 데미지(%)")]
        [InfoBox("기존 데미지의 %로 들어감" +
                 "\n 기존이 50%, 이 값이 50%일시 25% 데미지")]
        public float firstDmg = 100;

        [ShowIf("atkTypeEnum", AttackObject.AttackTypeEnum.Tick)] [TabGroup("공격 오브젝트 설정")] [LabelText("첫 공격 그로기(%)")]
        public float firstGroggy = 100;

        [ShowIf("atkTypeEnum", AttackObject.AttackTypeEnum.Delay)] [TabGroup("공격 오브젝트 설정")] [LabelText("딜레이 시간")]
        public float delayTime;

        [ShowIf("atkTypeEnum", AttackObject.AttackTypeEnum.Cd)] [TabGroup("공격 오브젝트 설정")] [LabelText("쿨타임")]
        public float cd;

        [TabGroup("투사체 설정")] [LabelText("중력 크기")]
        public float gravityScale;

        [TabGroup("투사체 설정")] [LabelText("가속도")] [Tooltip("[unit/s^2]")]
        public float acceleration;

        [TabGroup("투사체 설정")] [LabelText("최대 이동거리")] [Tooltip("parabolic 투사체에선 적용되지 않습니다.")]
        public float maxDistance;

        [FormerlySerializedAs("mapConflictType")] [TabGroup("투사체 설정")] [LabelText("벽 충돌타입")]
        public ProjectileConflictType wallConflictType;

        [TabGroup("투사체 설정")] [LabelText("바닥 충돌타입")]
        public ProjectileConflictType groundConflictType;

        [TabGroup("투사체 설정")] [LabelText("타겟 충돌타입")]
        public ProjectileConflictType targetConflictType;

        [TabGroup("투사체 설정")][LabelText("보스 충돌타입 사용여부")] public bool useBossConflict;
        [TabGroup("투사체 설정")] [LabelText("보스 충돌타입")][ShowIf("useBossConflict")]
        public ProjectileConflictType bossConflictType;
        [ShowIf("targetConflictType", ProjectileConflictType.Penetrate)] [TabGroup("투사체 설정")] [LabelText("관통 최대 횟수")]
        public int penetrationMax;

        [ShowIf("targetConflictType", ProjectileConflictType.Penetrate)]
        [TabGroup("투사체 설정")]
        [LabelText("관통당 데미지 증가량")]
        [Tooltip("감소 시 마이너스로 기입")]
        public int penetrationDmg;

        [ShowIf("targetConflictType", ProjectileConflictType.Penetrate)]
        [TabGroup("투사체 설정")]
        [LabelText("관통당 그로기 증가량")]
        [Tooltip("감소 시 마이너스로 기입")]
        public int penetrationGroggy;

        [ShowIf("targetConflictType", ProjectileConflictType.Penetrate)]
        [TabGroup("투사체 설정")]
        [LabelText("관통당 크기 증가량(%)")]
        [Tooltip("감소 시 마이너스로 기입")]
        public float penetrationRadius;

        [TabGroup("투사체 설정")] [LabelText("초기 속도")] [Tooltip("초기 방향이 달라진다면 발사 속력으로 간주")]
        public Vector2 firstVelocity;

        [TabGroup("투사체 설정")] [LabelText("방향 = 속도vector 여부")]
        public bool isRotateToVelocity = true;

        [ShowIf("isRotateToVelocity")] [TabGroup("투사체 설정")] [LabelText("투사체가 바라볼 방향")]
        public Projectile.Dir projDirection;

        [TabGroup("투사체 설정")] [LabelText("좌우 방향에 따른 y flip 여부")]
        public bool isYFlipByDirection = true;

        [TabGroup("투사체 설정")] [LabelText("속도 0일시 Destroy 여부")]
        public bool destroyWhenZero;


        [TabGroup("유도투사체 설정")] [LabelText("유도 파워")]
        public float followPower;

        [TabGroup("유도투사체 설정")] [LabelText("유도 범위")]
        public float followRange;

        [TabGroup("유도투사체 설정")] [LabelText("타겟 설정 타입")]
        public GuideTargetType guideTargetType = GuideTargetType.FoundUntilFirst;

        [TabGroup("유도투사체 설정")] [LabelText("타겟 찾기 원뿔각도")] [Tooltip("0으로 설정하면 그냥 원으로 계산")]
        public float targetFoundAngle = 0;

    }
}