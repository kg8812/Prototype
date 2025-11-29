using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Player
{
    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("대시 쿨타임")] [SerializeField]
    private float dashCoolTime; //대시 쿨타임

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("점프 후 공격 딜레이")] [SerializeField]
    private float jumpToAttackDelay; //점프 후 공격까지 딜레이 시간

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("회피 후 공격 딜레이")] [SerializeField]
    private float dashToAttackDelay; // 회피 후 공격까지 딜레이 시간

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("회피 후 이동 딜레이")] [SerializeField]
    private float dashToMoveDelay = 0.3f;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("회피 후 점프 딜레이")] [SerializeField]
    private float dashToJumpDelay = 0.01f; // 회피 후 공격까지 딜레이 시간

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("지상 회피 후 착지 시간")] [SerializeField]
    private float dashLandingTime = 0.6f;


    [HideInInspector] public PlayerStat playerStat;

    private BonusStat _levelStat;

    private PlayerStatManager playerStatManager;
    public override StatManager StatManager => playerStatManager ??= new PlayerStatManager(_statManager);

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

    private BonusStat LevelBonusStat() //레벨업시 스탯 변경
    {
        _levelStat ??= new BonusStat();
        _levelStat.Reset();

        return _levelStat;
    }

    public void ResetPlayerStatus()
    {
        animator.Rebind();
        IdleOn();
        CurHp = MaxHp;
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

    public void SetUnitData(UnitData _unitData)
    {
        if (_unitData == null) return;
        ActiveSkill?.UnEquip();
        PassiveSkill?.UnEquip();
        StatManager.BaseStat = new BaseStat(_unitData.baseStat);
        ActiveSkill = _unitData.activeSkill;
        PassiveSkill = _unitData.passiveSkill;
        UpdatePlayerStat(_unitData);
        UpdateSkills();
    }

    public void UpdatePlayerStat(UnitData _unitData)
    {
        playerStat = new PlayerStat(_unitData.playerStat);
    }

    public void UpdatePlayerStat()
    {
    }

    #region 입력 버퍼

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("액션 커맨드 최대 버퍼 수")] [SerializeField]
    private int maxActionBufferSize = 4;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("방향 커맨드 최대 버퍼 수")] [SerializeField]
    private int maxDirectionBufferSize = 1;

    public int MaxActionBufferSize => maxActionBufferSize;
    public int MaxDirectionBufferSize => maxDirectionBufferSize;

    #endregion

    #region 물리 관련

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("최대 이동 속도")] [SerializeField]
    private float maxMoveVel = 7f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("이동 저항 정도")] [SerializeField]
    private float moveResistFactor = 2.5f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("사전 동작 최대 이동 속도")] [SerializeField]
    private float preattackMoveSpeed = 2f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("최대 낙하 속도")] [SerializeField]
    private float initMaxDropVel = 7f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("낙하 저항 정도")] [SerializeField]
    private float dropResistFactor = 1f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("공중 공격 낙하 속도")] [SerializeField]
    private float attackDropMaxVel = 1f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("공중 정지 저항 정도")] [SerializeField]
    private float airStopResistFactor = 2.2f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("점프 세기")] [SerializeField]
    private float jumpForce = 7f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("점프 코요테 타임")] [SerializeField]
    private float jumpCoyoteTime = 0.3f;

    [TabGroup("기획쪽 수정 변수들/group1", "물리")] [LabelText("달리기 속도")] [SerializeField]
    private float runVel = 8f;

    public float MaxMoveVel => maxMoveVel;
    public float PreattackMoveSpeed => preattackMoveSpeed;
    public float MoveResistFactor => moveResistFactor;
    public float MaxDropVel { get; private set; }

    public float DropResistFactor => dropResistFactor;
    public float AttackDropMaxVel => attackDropMaxVel;
    public float AirStopResistFactor => airStopResistFactor;
    public float JumpForce => playerStat.JumpPower;
    public float RunVel => runVel;

    #endregion
}