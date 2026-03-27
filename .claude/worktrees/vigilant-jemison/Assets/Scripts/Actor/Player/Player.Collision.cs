using Default;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Player : IPhysicsTransition
{
    [TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")] [LabelText("몬스터 충돌(입력 종료) 밀려나는 세기")] [SerializeField]
    private float resistForce;

    [TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")]
    [LabelText("몬스터 충돌(중심 이동) 밀려나는 세기")]
    [Tooltip("범위: 0.01 ~ 1")]
    [Range(0.01f, 1f)]
    [SerializeField]
    private float dragFactor;

    [TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")] [LabelText("피격 무적 시간")] [SerializeField]
    public float hitInvincibleTime;

    [TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")] [LabelText("넉백 시간")] [SerializeField]
    public float knockBackTime = 1;

    [TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")] [LabelText("넉백 세기")] [SerializeField]
    public float knockBackForce = 2;

    [TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")] [LabelText("넉백 각도")] [SerializeField]
    public float knockBackAngle = 20; // DEG

    [TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")] [LabelText("기본 넉백 데이터")] [SerializeField]
    public KnockBackData knockBackData = new()
    {
        knockBackType = KnockBackData.KnockBackType.Default,
        directionType = KnockBackData.DirectionType.AbsoluteAngle,
        symmetryType = KnockBackData.SymmetryType.None,
        knockBackTime = 1,
        knockBackForce = 2,
        knockBackAngle = 20
    };

    private ActorPhysicsTransitionHandler _physicsTransitionHandler;

    public float ResistForce => resistForce;

    public float DragFactor => dragFactor;

    public ActorPhysicsTransitionHandler PhysicsTransitionHandler => _physicsTransitionHandler ??=
        gameObject.GetOrAddComponent<ActorPhysicsTransitionHandler>();

    protected override void OnHitReaction(EventParameters eventParameters)
    {
        if (GetState() == EPlayerState.Dead) return;

        // base는 점멸효과만 발생
        base.OnHitReaction(eventParameters);
    }
}