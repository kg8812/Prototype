using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Define
{
    #region Enums

    public enum GameKey
    {
        LeftMove,
        RightMove,
        Down,
        Up,
        Jump,
        Attack,
        ActiveSkill,
        
    }

    public enum UIKey
    {
        LeftHeader,
        RightHeader,
        Left,
        Right,
        Up,
        Down,
        Select,
        Cancel,
        Equip,
        Skip,
    }

    public enum UIEvent
    {
        Click,
        BeginDrag,
        Drag,
        EndDrag,
        Drop,
        PointDown,
        PointUp,
        PointEnter,
        PointExit,
        PointStay
    }

    public enum Sound
    {
        BGM,
        SFX,
        UI,
        Ambience,
        Master,
        MaxCOUNT
    }

    public enum AttackType
    {
        Extra,
        BasicAttack,
    }

    #endregion


    #region Address 모음

    #region 사운드

    public struct BGMList
    {
        public const string MainThemeTitle = "MainThemeTitle";
        public const string MainTheme = "MainTheme";
    }

    public struct SFXList
    {
    }

    #endregion

    #region 이펙트

    public struct PlayerEffect // 플레이어 이펙트
    {
    }

    public struct EtcEffects
    {
    }

    public static class BossEffects
    {
    }

    public static class DummyEffects
    {
        public const string Explosion = "DummyExplosion";
        
    }

    #endregion

    #region 오브젝트

    public static class PlayerSkillObjects
    {
    }

    public static class CommonObjects
    {
    }

    public static class AccessoryObjects
    {
    }

    public static class BossObjects
    {
    }

    public static class StageObjects
    {
    }

    #endregion

    #region 데이터

    public class PlayerData
    {
    }

    #endregion

    #region 씬이름

    public class SceneNames
    {
        public const string TitleSceneName = "Title";
        public const string LobbySceneName = "Lobby";
        public const string MainWorldSceneName = "MainWorld";
    }

    #endregion

    #endregion
}


#region 효과 및 스탯관련 Enums

public enum ValueType
{
    Value,
    Ratio
}

public enum ActorStatType // 스탯 종류
{
    Atk = 0,
    Def,
    AtkSpeed,
    MoveSpeed,
    MaxHp,
    CDReduction,
}

public enum EventType // 효과 적용 조건
{
    OnEnable, // 켜질 때
    OnDisable, // 꺼질 때
    OnColliderAttack, // 근거리 공격 시 (콜라이더로 공격할 때)
    OnWeaponSlash, // 무기 휘두를 때
    OnAttack, // 공격 휘두를 때 (안맞아도됨)
    OnAttackSuccess, // 공격 성공 시
    OnBasicAttack, // 기본 공격 성공시
    OnAfterAtk, // 공격 후 (입힌 데미지 확인해야될 때)
    OnBackAttack, // 백어택
    OnAttackEnd, // 공격 모션 끝날 때
    OnJump, // 점프 시
    OnBeforeHit, // 피격 직전 (무적, 방어 등)
    OnHit, // 피격시
    OnHitReaction, // 피격 효과 발동시 (플레이어)
    OnAfterHit, // 피격 시 (데미지 증감이 적용된 후 피격)
    OnHpDown, // 체력 감소 시
    OnKill, // 적 처치 시
    OnSkill, // 고유 스킬 사용 시
    OnSkillEnd, // 고유 스킬 종료 시
    OnSubBuffTaken, // 서브버프 받을 시
    OnSubBuffRemove, // 서브버프 제거될 시
    OnDash, // 회피 사용 시
    OnRepair, // 수리 사용 시
    OnCrit, // 크리티컬 데미지 시
    OnCritHit, // 크리티컬 피격 시
    OnDeath, // 사망시
    OnTriggerEnter, // 충돌시
    OnTriggerExit, // 충돌 해제시 
    OnSubBuffApply, // 적에게 서브버프 적용시
    OnUpdate, // 업데이트
    OnFixedUpdate, // 물리 업데이트
    OnWeaponSkillUse, // 무기 스킬 사용시
    OnDestroy, // 파괴시
    OnRecognitionEnter, // 인식 상태 돌입 (몬스터)
    OnRecognitionExit, // 인식 상태 탈출 (몬스터)
    OnStateChanged, // 상태 변환 (몬스터)
    OnBuffGroupAdd, // 효과 장착시
    OnBuffGroupRemove, // 효과 해제시 
    OnHpHeal, // 체력 회복시
    OnCollide, // 충돌시
    OnRepairCharge,
    OnMove,
    OnBeforeHpDown, // 체력 감소 직전
    OnBarrierChange, // 쉴드값 변경시
    OnAttackStateEnter, // 공격상태 진입시
    OnAttackStateExit, // 공격상태 해제시
    OnInit, // 초기화시
    OnCutScene,
    OnTargetFirstAttack, // 각 타겟을 첫 공격시 (타겟마다 호출됨)
    OnFirstAttack, // 첫 타격시 (공격당 한번만 적용)
    OnChargeEnd, // 차징 완료 시
    OnCastingEnd, // 캐스팅 완료 시
    OnChargeCancel, // 차징 캔슬 시
    OnCastingCancel, // 캐스팅 캔슬 시
    OnLanding, // 바닥 착지 시
    OnAirEnter, // 공중 진입 시
    OnIdle, // idle state 진입 시
    OnStop, // 이동 멈출 시
    OnEventState, // EventState 전환 시
    OnIdleMotion,
    OnTurn,
    OnKnockbackComplete,
    OnClimbStart,
    OnClimbMotionStart,
    OnClimbEnd,
    OnAnyState,
    OnCutSceneEnd,
    OnDrillComplete,
}

#region 버프,디버프 관련

public enum SubBuffType // 버프 종류, 클래스랑 이름 똑같이 해야함
{
    Buff_DoubleJump,
    Debuff_SequenceStack,
    Buff_Warmth,
    MarkStack,
    StarEye,
    Celerity,
    HunterStack,
    RefinedAnger,
    DisCharge,
    ExpansionBullet,

    Buff_Atk,
    Buff_Def,
    Buff_AtkSpeed,
    Buff_MoveSpeed,
    Buff_CritProb,
    Buff_CritDmg,
    Buff_CD,
    Buff_Mentality,
    Buff_MaxHp,
    Buff_DmgReduce,
    Buff_ExtraDmg,
    Buff_GoldRate,
    Buff_HealRate,
    Buff_ShieldRate,
    Buff_AllStat,

    Debuff_Atk,
    Debuff_Def,
    Debuff_AtkSpeed,
    Debuff_MoveSpeed,
    Debuff_CritProb,
    Debuff_CritDmg,
    Debuff_CD,
    Debuff_Mentality,
    Debuff_MaxHp,
    Debuff_DmgReduce,
    Debuff_ExtraDmg,
    Debuff_GoldRate,
    Debuff_HealRate,
    Debuff_ShieldRate,

    Buff_BasicAtkEnhance,

    Debuff_Stun,
    Debuff_Chill,
    Debuff_Frozen,
    Debuff_Grab,
    Debuff_KnockBack,
    Debuff_BonusDmg,
    Debuff_Execution,
    Debuff_Tied,

    Buff_Immune,
    Buff_Remove,

    Debuff_Burn,
    Debuff_Poison,
    Debuff_Bleed,
    Debuff_RasenganResidue,

    Buff_MaxHpRegen,
    Buff_LostHpRegen,
    Buff_HpRegen,
    Buff_LifeSteal,
    Buff_ShieldHpRegen,

    Buff_Barrier,
    Buff_DefBarrier,

    Debuff_MaxHpLose,
    Debuff_LostHpLose,
    Debuff_HpLose,
    Debuff_AtkHpLose,
    Debuff_CurHpLose,
    Debuff_GroggyLose,

    Buff_Sight,
    Buff_AirDashCount,
    Buff_RepairValue,
    Buff_RepairCount,
    Buff_WeaponCD,
    Buff_SkillDmg,
    Buff_DashRange,

    Buff_ActiveDuration,
    Buff_ActiveCD,
    Buff_ActiveCurCD,

    None
}

#endregion

#region 플레이어 스테이트 관련

public static class NextState
{
    public static EPlayerState[] Get(EPlayerState currentState)
    {
        return currentState switch
        {
            EPlayerState.Idle => new[]
            {
                EPlayerState.AirIdle, EPlayerState.Move, EPlayerState.Jump, EPlayerState.Attack, EPlayerState.Dash,
                EPlayerState.Heal, EPlayerState.Skill, EPlayerState.Crouch, EPlayerState.Drop, EPlayerState.Heal
            },
            EPlayerState.AirIdle => new[]
            {
                EPlayerState.Idle, EPlayerState.AirMove, EPlayerState.Jump, EPlayerState.Attack, EPlayerState.Dash,
                EPlayerState.Skill, EPlayerState.Climb
            },
            EPlayerState.Move => new[]
            {
                EPlayerState.AirIdle, EPlayerState.AirMove, EPlayerState.Jump, EPlayerState.Attack, EPlayerState.Dash,
                EPlayerState.Heal, EPlayerState.Skill, EPlayerState.Stop, EPlayerState.Drop, EPlayerState.Heal
            },
            EPlayerState.AirMove => new[]
            {
                EPlayerState.AirIdle, EPlayerState.Move, EPlayerState.Jump, EPlayerState.Attack, EPlayerState.Dash,
                EPlayerState.Skill, EPlayerState.Stop, EPlayerState.Climb
            },
            EPlayerState.Crouch => new[]
                { EPlayerState.Idle, EPlayerState.Drop, EPlayerState.Move, EPlayerState.Heal, EPlayerState.Jump },
            EPlayerState.Dash => new[] { EPlayerState.Jump, EPlayerState.Climb },
            EPlayerState.DashLanding => new[] { EPlayerState.Jump },
            EPlayerState.Stop => new[] { EPlayerState.Attack },
            EPlayerState.AttackWaiting => new[] { EPlayerState.Dash },
            EPlayerState.AirAttackWaiting => new[] { EPlayerState.Dash },
            EPlayerState.Run => new[]
            {
                EPlayerState.Run, EPlayerState.AirIdle, EPlayerState.AirMove, EPlayerState.Jump, EPlayerState.Attack,
                EPlayerState.Dash, EPlayerState.Heal, EPlayerState.Skill, EPlayerState.Stop, EPlayerState.Drop,
                EPlayerState.Heal, EPlayerState.Move
            },
            EPlayerState.AirRun => new[]
            {
                EPlayerState.Run, EPlayerState.AirIdle, EPlayerState.Move, EPlayerState.Jump, EPlayerState.Attack,
                EPlayerState.Dash, EPlayerState.Skill, EPlayerState.Stop, EPlayerState.Climb
            },
            EPlayerState.IceDrillCharge => new[]
            {
                EPlayerState.IceDrillCharge, EPlayerState.AirIdle, EPlayerState.AirMove, EPlayerState.Jump,
                EPlayerState.Attack, EPlayerState.Heal, EPlayerState.Skill, EPlayerState.Stop, EPlayerState.Drop,
                EPlayerState.Heal, EPlayerState.Move
            },
            // EPlayerState.Damaged => new[] { EPlayerState.Attack, EPlayerState.Skill },
            _ => null
        };
    }
}

#endregion

#region 플레이어 애니메이터

public enum EAnimationBool
{
    Idle,
    IsMove,
    Turn,
    OnAir,
    IsCrouch,
    IsDash,
    IsFrontKnockback,
    IsRun,
    IsIceDrill,
}

public enum EAnimationTrigger
{
    Climb,
    ClimbLow,
    Turn,
    Jump,
    Attack,
    Heal,
    Damaged,
    Dead,
    Interact,
    IdleOn,
    IdleFix,
    Stop,
    Dash,
    AttackInit,
    PreAttack,
    StepAttack,
    KnockbackEnter,
    KnockbackEnd,
    IdleFixOff,
    InteractEnd,
    CutsceneOn,
    CutsceneEnd,
    IceDrillOn,
    Drop,
}

public enum EAnimationInt
{
    Direction,
    MaxGroundAtk,
    MaxAirAtk,
    AttackType,
    WeaponSkillType,
    DashType,
    ChargingType,
}

public enum EAnimationFloat
{
    AtkSpeed,
    MoveMultiplier,
    PotionTime,
    MoveSpeed,
}

#endregion

#endregion