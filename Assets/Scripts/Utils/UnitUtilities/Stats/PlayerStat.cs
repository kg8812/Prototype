using System;
using Sirenix.OdinInspector;

[Serializable]
public struct PlayerStat
{
    [LabelText("점프 뛰는 힘")] public float JumpPower; // 점프 뛰는 힘

    [LabelText("점프 최대 횟수")] public int JumpMax; // 점프 최대치

    [LabelText("대시 지속시간")] public float dashTime; //대시 지속시간

    [LabelText("대시 속도")] public float dashSpeed; //대시 속도

    [LabelText("대시 무적시간")] public float dashInvincibleTime; //대시 무적 시간

    [LabelText("공중 대시 횟수")] public int airDashCount;


    public PlayerStat(PlayerStat other)
    {
        JumpPower = other.JumpPower;
        JumpMax = other.JumpMax;
        dashTime = other.dashTime;
        dashSpeed = other.dashSpeed;
        dashInvincibleTime = other.dashInvincibleTime;
        airDashCount = other.airDashCount;
    }

    public static PlayerStat operator +(PlayerStat a, PlayerStat b)
    {
        var c = new PlayerStat(a);
        c.JumpPower += b.JumpPower;
        c.JumpMax += b.JumpMax;
        c.dashTime += b.dashTime;
        c.dashSpeed += b.dashSpeed;
        c.dashInvincibleTime += b.dashInvincibleTime;
        c.airDashCount += b.airDashCount;
        return c;
    }

    public static PlayerStat operator -(PlayerStat a, PlayerStat b)
    {
        var c = new PlayerStat(a);
        c.JumpPower -= b.JumpPower;
        c.JumpMax -= b.JumpMax;
        c.dashTime -= b.dashTime;
        c.dashSpeed -= b.dashSpeed;
        c.dashInvincibleTime -= b.dashInvincibleTime;
        c.airDashCount -= b.airDashCount;
        return c;
    }
}