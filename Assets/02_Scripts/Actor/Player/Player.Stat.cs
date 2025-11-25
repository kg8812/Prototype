using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using Apis;
using Save.Schema;
using UI;

public partial class Player
{
    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("대시 쿨타임")][SerializeField] float dashCoolTime; //대시 쿨타임

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("점프 후 공격 딜레이")]
    [SerializeField] float jumpToAttackDelay; //점프 후 공격까지 딜레이 시간

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("대쉬 후 공격 딜레이")]
    [SerializeField] float dashToAttackDelay; // 대쉬 후 공격까지 딜레이 시간

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("대쉬 후 이동 딜레이")]
    [SerializeField] float dashToMoveDelay = 0.3f;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("대쉬 후 점프 딜레이")]
    [SerializeField] float dashToJumpDelay = 0.01f; // 대쉬 후 공격까지 딜레이 시간

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("지상 대쉬 후 착지 시간")]
    [SerializeField] float dashLandingTime = 0.6f;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("얼음 드릴 진입 시간")]
    [SerializeField] float iceDirllEnterTime = 1f;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("얼음 드릴 발동 후 후딜레이")]
    [SerializeField] float iceDirllAfterDelay = 1f;

    private PlayerStatManager playerStatManager;
    public override StatManager StatManager => playerStatManager ??= new(_statManager);


    [HideInInspector]
    public PlayerStat playerStat;
    
    public float PotionUseTime => playerStat.potionUseTime;

    public float PotionIncreaseHp
    {
        get => playerStat.potionIncreaseHp;
        set => playerStat.potionIncreaseHp = value;
    }

    public int CurrentPotionCapacity => playerStat.currentPotionCapacity;

    public int MaxPotionCapacity => playerStat.maxPotionCapacity;

    public float DashTime => playerStat.dashTime;

    public float DashSpeed
    {
        get => playerStat.dashSpeed;
        set => playerStat.dashSpeed = value;
    }

    public float DashInvincibleTime => playerStat.dashInvincibleTime;

    public float DashCoolTime => dashCoolTime;

    public float JumpAttackCoolTime => jumpToAttackDelay;

    public float DashAttackCoolTime => dashToAttackDelay;

    public float DashToMoveDelay => dashToMoveDelay;

    public float DashLandingTime => dashLandingTime;

    public float DashToJumpDelay => dashToJumpDelay;
    public float IceDrillEnterTime => iceDirllEnterTime;
    public float IceDirllAfterDelay => iceDirllAfterDelay;

    [HideInInspector] public float repairRatio;
    [HideInInspector] public UnityEvent<int> OnPotionChange;
    
    private BonusStat _levelStat;
    
    BonusStat LevelBonusStat() //레벨업시 스탯 변경
    {
        _levelStat ??= new();
        _levelStat.Reset();

        return _levelStat;
    }
    public void ResetPlayerStatus()
    {
        animator.Rebind();
        IdleOn();
        CurHp = MaxHp;
        increasePotionCapacity(MaxPotionCapacity);
        if (ActiveSkill != null)
        {
            ActiveSkill.Cancel();
            ActiveSkill.CurCd = 0;
        }

        if (PassiveSkill != null)
        {
            PassiveSkill.Cancel();
            PassiveSkill.CurCd = 0;
        }
    }
    public void increasePotionCapacity(int amount)
    {
        playerStat.currentPotionCapacity += amount;
        if (playerStat.currentPotionCapacity > playerStat.maxPotionCapacity)
        {
            playerStat.currentPotionCapacity = playerStat.maxPotionCapacity;
        }
        else if (playerStat.currentPotionCapacity < 0)
        {
            playerStat.currentPotionCapacity = 0;
        }
        OnPotionChange.Invoke(playerStat.currentPotionCapacity);
        
        if (amount >= 1)
        {
            ExecuteEvent(EventType.OnRepairCharge, new EventParameters(this));
        }
    }
    public void IncreaseMaxPotion(int amount)
    {
        playerStat.maxPotionCapacity += amount;
    }

    public void SetUnitData(UnitData _unitData)
    {
        if (_unitData == null) return;
        activeSkill?.UnEquip();
        passiveSkill?.UnEquip();
        Mecanim.skeletonDataAsset = _unitData.skeletonDataAsset;
        Mecanim.initialSkinName = _unitData.initialSkinName;
        Mecanim.Initialize(true);
        StatManager.BaseStat = new(_unitData.baseStat);
        activeSkill = _unitData.activeSkill;
        passiveSkill = _unitData.passiveSkill;
        UpdatePlayerStat(_unitData);
        _overrider?.Init(this);
        UpdateSkills();
    }

    public void UpdatePlayerStat(UnitData _unitData)
    {
        playerStat = new(_unitData.playerStat);
    }

    public void UpdatePlayerStat()
    {
    }
#region 입력 버퍼
    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("액션 커맨드 최대 버퍼 수")][SerializeField] int maxActionBufferSize = 4;
    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("방향 커맨드 최대 버퍼 수")][SerializeField] int maxDirectionBufferSize = 1;
    public int MaxActionBufferSize => maxActionBufferSize;
    public int MaxDirectionBufferSize => maxDirectionBufferSize;
#endregion

#region 물리 관련
    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("최대 이동 속도")][SerializeField] float maxMoveVel = 7f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("이동 저항 정도")][SerializeField] float moveResistFactor = 2.5f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("사전 동작 최대 이동 속도")][SerializeField] float preattackMoveSpeed = 2f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("최대 낙하 속도")][SerializeField] float initMaxDropVel = 7f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("낙하 저항 정도")][SerializeField] float dropResistFactor = 1f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("공중 공격 낙하 속도")][SerializeField] float attackDropMaxVel = 1f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("공중 정지 저항 정도")][SerializeField] float airStopResistFactor = 2.2f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("점프 세기")][SerializeField] float jumpForce = 7f;
    
    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("점프 코요테 타임")][SerializeField] float jumpCoyoteTime = 0.3f;
    [TabGroup("기획쪽 수정 변수들/group1", "물리")]
    [LabelText("달리기 속도")][SerializeField] float runVel = 8f;

    private float maxDropVel;

    public float MaxMoveVel => maxMoveVel;
    public float PreattackMoveSpeed => preattackMoveSpeed;
    public float MoveResistFactor => moveResistFactor;
    public float MaxDropVel => maxDropVel;
    public float DropResistFactor => dropResistFactor;
    public float AttackDropMaxVel => attackDropMaxVel;
    public float AirStopResistFactor => airStopResistFactor;
    public float JumpForce => playerStat.JumpPower;
    public float RunVel => runVel;
#endregion
}
