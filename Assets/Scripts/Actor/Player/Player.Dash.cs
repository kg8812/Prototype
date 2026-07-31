public partial class Player : Actor
{
    public void ForceDash(float velocity, EDirection direction)
    {
        MoveComponent.ForceActorMovement.Dash(velocity, direction);
    }

    #region 대쉬 전략

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

    #endregion
}
