using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Player : Actor
{
    public void ForceDash(float velocity, EDirection direction)
    {
        MoveComponent.ForceActorMovement.Dash(velocity, direction);
    }

    #region 대쉬

    private IPlayerDash dashStrategy;
    public IPlayerDash DashStrategy => dashStrategy ?? new BasicDash(this);

    public void SetDash(IPlayerDash dash)
    {
        dashStrategy = dash;
        animator.SetInteger("DashType", dash.MotionType());
    }

    public void SetDashToNormal()
    {
        SetDash(new BasicDash(this));
    }

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("회피 속도 그래프")] [SerializeField]
    private Ease dodgeSpeedGraph = Ease.OutSine;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("회피 후딜레이 속도 그래프(deprecated)")] [SerializeField]
    private Ease dashDelaySpeedGraph = Ease.OutSine;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("회피 후딜레이 이동거리(deprecated)")] [SerializeField]
    private float dashDelayDistance = 0.7f;

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("회피 캔슬 시간")] [SerializeField]
    private float dashDelayCancelTime = 0.3f;

    public Ease DodgeSpeedGraph => dodgeSpeedGraph;
    public Ease DashDelaySpeedGraph => dashDelaySpeedGraph;
    public float DashDelayDistance => dashDelayDistance;
    public float DashDelayCancelTime => dashDelayCancelTime;

    #endregion
}