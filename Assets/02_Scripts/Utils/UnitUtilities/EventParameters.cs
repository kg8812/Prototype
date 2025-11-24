using System;
using System.Collections.Generic;
using Apis;
using Apis;
using Apis.CommonMonster2;
using EventData;
using UnityEngine;

public class EventParameters
{
    // 이벤트 관련 파라미터 목록
    
    public readonly IEventUser user; // 효과 사용자
    public IEventUser master; // 실 사용자 (이벤트 사용을 무기가 했을 시, 무기를 장착한 플레이어)
                              // 무기로 이벤트를 사용했을 때 플레이어에 적용되는 효과들이 있어서 필요함

    public IOnHit target; // 공격하는 타겟
    
    public MonsterState monsterState; // common monster에서만 씀.

    // 데이터 목록

    public AttackEventData atkData; // 공격 관련 파라미터
    public KnockBackData knockBackData; // 넉백 관련 파라미터
    public KnockBackData groggyKnockBackData; // 그로기넉백 관련 파라미터
    public HitEventData hitData; // 피격 관련 파라미터
    public BuffEventData buffData; // 버프 관련 파라미터
    public StatEventData statData; // 스탯 관련 파라미터
    public CollideEventData collideData; // 충돌 관련 파라미터
    public SkillEventData skillData; // 스킬 관련 파라미터

   
    public EventParameters(IEventUser user)
    {
        this.user = user;
        master = user;
    }
    public EventParameters(IEventUser user, IOnHit target)
    {
        this.user = user;
        master = user;
        this.target = target;
    }

    public void Reset()
    {
        atkData.Reset();
        knockBackData.Reset();
        groggyKnockBackData.Reset();
        hitData.Reset();
        buffData.Reset();
        statData.Reset();
        collideData.Reset();
        skillData.Reset();
    }
}



namespace EventData
{
    public interface IReset
    {
        public void Reset();
    }
    
    [System.Serializable]
    public struct KnockBackData : IReset
    {
        public enum KnockBackType
        {
            Default, Groggy // 일반 넉백, 그로기 넉백
        }

        public enum SymmetryType
        {
            None,             // 기본
            Vertical,         // y축 대칭
            Horizontal,       // x축 대칭
            Point             // 점 대칭
        }

        public enum DirectionType
        {
            AttackerRelative,   // attacker 기준 방향
            AktObjRelative,     // attackObject 기준 방향
            AbsoluteAngle       // 절대 각도 기준 방향
        }

        public KnockBackType knockBackType; // 해당 공격의 넉백 타입
        public float knockBackForce; // 해당 공격의 넉백 파워
        public float knockBackTime; // 해당 공격의 넉백 타임(날라가는 시간)
        public float knockBackAngle; // 넉백 각도
        public SymmetryType symmetryType;   // 해당 공격의 넉백 대칭 종류
        public DirectionType directionType; // 넉백 방향 계산 종류

        public void Reset()
        {
            knockBackType = KnockBackType.Default;
            knockBackForce = 0;
            knockBackTime = 0; 
            knockBackAngle = 0;
            symmetryType = SymmetryType.None;
            directionType = DirectionType.AttackerRelative;
        }
    }
    public struct AttackEventData : IReset
    {
        public enum HitReactionType{
            Base, Stuck, Knockback
        }
        public float dmg; // 입힐 데미지 (계산용)
        public int groggyAmount; // 그로기 수치
        public bool isHitReaction; // 피격 반응 있는 공격인가? <- knockbackData로 옮겨짐.
        public IAttackStrategy atkStrategy; // 데미지 적용 방식 (공격력 비례 등)
        public Define.AttackType attackType; // 공격 타입 (기본, 스킬 등)
        public Guid attackGuid; // 이 공격을 생성한 공격 혹은 스킬 guid (같은 스킬에서 생성된건지 확인여부)
        public HitReactionType hitReactionType; // 공격 reaction 유형. 나중에 isHitReaction 없어질 예정.

        public bool isfixedCrit; // 무조건 치명타 공격일시
        
        public void Reset()
        {
            dmg = 0;
            groggyAmount = 0;
            isHitReaction = false;
            atkStrategy = new FixedAmount(0);
            attackType = Define.AttackType.Extra;
            attackGuid = Guid.Empty;
            hitReactionType = HitReactionType.Base;
        }
    }

    public struct HitEventData : IReset
    {
        public float dmg; // 받을 데미지
        public bool isHitContinue; // 피격 모션 고정시킬지 여부(true인 경우 피격 모션 해제는 Player.IdleOn으로 따로 해줘야 함)
        public bool hitDisable; // 피격해제 여부 (true시 데미지 안받음)
        public bool isCritApplied; // 크리티컬 적용 여부
        public float dmgReceived; // 최종적으로 입은 데미지 (데미지 계산이 끝난 후 최종값을 입력)
        public void Reset()
        {
            dmg = 0;
            isHitContinue = false;
            hitDisable = false;
            isCritApplied = false;
            dmgReceived = 0;
        }
    }

    public struct BuffEventData: IReset
    {
        public int buffGroupId; // 버프 그룹 ID

        public SubBuff activatedSubBuff; // 적용시킨 버프
        public SubBuff removedSubBuff; // 제거된 버프
        public SubBuff takenSubBuff; // 받은 버프

        public void Reset()
        {
            buffGroupId = 0;
            activatedSubBuff = null;
            removedSubBuff = null;
            takenSubBuff = null;
        }
    }

    public struct StatEventData : IReset
    {
        private BonusStat _stat;
        public BonusStat stat => _stat ??= new(); // 임시 추가 스탯
        public void Reset()
        {
            stat.Reset();
        }
    }

    public struct CollideEventData : IReset
    {
        public Collider2D collider; // 충돌한 콜라이더
        public void Reset()
        {
            collider = null;
        }
    }

    public struct SkillEventData : IReset
    {
        public Skill usedSkill; // 사용한 스킬
        public void Reset()
        {
            usedSkill = null;
        }
    }
}