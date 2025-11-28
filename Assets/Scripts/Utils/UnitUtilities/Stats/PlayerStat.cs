using System;
using Sirenix.OdinInspector;

[Serializable]
public struct PlayerStat
{
    [LabelText("점프 뛰는 힘")] public float JumpPower; // 점프 뛰는 힘

    [LabelText("점프 최대 횟수")] public int JumpMax; // 점프 최대치

    [LabelText("현재 포션 개수")] public int currentPotionCapacity; //현재 포션 충전량

    [LabelText("대시 지속시간")] public float dashTime; //대시 지속시간

    [LabelText("대시 속도")] public float dashSpeed; //대시 속도

    [LabelText("대시 무적시간")] public float dashInvincibleTime; //대시 무적 시간

    [LabelText("공중 대시 횟수")] public int airDashCount;

    [LabelText("포션 최대 개수")] public int maxPotionCapacity; //최대 포션 충전량

    [LabelText("포션 사용시간")] public float potionUseTime; //포션 사용시간

    [LabelText("포션 회복량 %")] public float potionIncreaseHpRatio; // 포션 hp 증가량 %

    [LabelText("포션 회복량 값")] public float potionIncreaseHp; //포션 hp 증가량

    public PlayerStat(PlayerStat other)
    {
        JumpPower = other.JumpPower;
        JumpMax = other.JumpMax;
        currentPotionCapacity = other.currentPotionCapacity;
        dashTime = other.dashTime;
        dashSpeed = other.dashSpeed;
        dashInvincibleTime = other.dashInvincibleTime;
        airDashCount = other.airDashCount;
        maxPotionCapacity = other.maxPotionCapacity;
        potionUseTime = other.potionUseTime;
        potionIncreaseHpRatio = other.potionIncreaseHpRatio;
        potionIncreaseHp = other.potionIncreaseHp;
    }

    public static PlayerStat operator +(PlayerStat a, PlayerStat b)
    {
        var c = new PlayerStat(a);
        c.JumpPower += b.JumpPower;
        c.JumpMax += b.JumpMax;
        c.currentPotionCapacity += b.currentPotionCapacity;
        c.dashTime += b.dashTime;
        c.dashSpeed += b.dashSpeed;
        c.dashInvincibleTime += b.dashInvincibleTime;
        c.airDashCount += b.airDashCount;
        c.maxPotionCapacity += b.maxPotionCapacity;
        c.potionUseTime += b.potionUseTime;
        c.potionIncreaseHpRatio += b.potionIncreaseHpRatio;
        c.potionIncreaseHp += b.potionIncreaseHp;
        return c;
    }

    public static PlayerStat operator -(PlayerStat a, PlayerStat b)
    {
        var c = new PlayerStat(a);
        c.JumpPower -= b.JumpPower;
        c.JumpMax -= b.JumpMax;
        c.currentPotionCapacity -= b.currentPotionCapacity;
        c.dashTime -= b.dashTime;
        c.dashSpeed -= b.dashSpeed;
        c.dashInvincibleTime -= b.dashInvincibleTime;
        c.airDashCount -= b.airDashCount;
        c.maxPotionCapacity -= b.maxPotionCapacity;
        c.potionUseTime -= b.potionUseTime;
        c.potionIncreaseHpRatio -= b.potionIncreaseHpRatio;
        c.potionIncreaseHp -= b.potionIncreaseHp;
        return c;
    }
}