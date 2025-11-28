public interface ISkill
{
    public SkillStat Stat { get; }
}

public class SkillStat
{
    public float baseCd;

    public float baseCdRatio;
    public float dmg;
    public float dmgRatio;
    public float duration;
    public float durationRatio;
    public int maxStack;
    public float maxStackRatio;
    public float Ratio;
    public int stackGain;

    public SkillStat()
    {
    }

    public SkillStat(float baseCd, int maxStack, float duration, float dmg)
    {
        this.baseCd = baseCd;
        this.maxStack = maxStack;
        this.duration = duration;
        this.dmg = dmg;
        baseCdRatio = 0;
        maxStackRatio = 0;
        durationRatio = 0;
        dmgRatio = 0;
        Ratio = 0;
    }

    public virtual void Reset()
    {
        baseCd = 0;
        maxStack = 0;
        duration = 0;
        dmg = 0;
        baseCdRatio = 0;
        maxStackRatio = 0;
        durationRatio = 0;
        dmgRatio = 0;
        Ratio = 0;
        stackGain = 0;
    }

    public virtual SkillStat Combine(SkillStat other)
    {
        SkillStat c;

        if (other == null)
            c = new SkillStat
            {
                baseCd = baseCd,
                maxStack = maxStack,
                duration = duration,
                dmg = dmg,
                stackGain = stackGain,
                baseCdRatio = baseCdRatio,
                maxStackRatio = maxStackRatio,
                durationRatio = durationRatio,
                dmgRatio = dmgRatio,
                Ratio = Ratio
            };
        else
            c = new SkillStat
            {
                baseCd = baseCd + other.baseCd,
                maxStack = maxStack + other.maxStack,
                duration = duration + other.duration,
                dmg = dmg + other.dmg,
                stackGain = stackGain + other.stackGain,
                baseCdRatio = baseCdRatio + other.baseCdRatio,
                maxStackRatio = maxStackRatio + other.maxStackRatio,
                durationRatio = durationRatio + other.durationRatio,
                dmgRatio = dmgRatio + other.dmgRatio,
                Ratio = Ratio + other.Ratio
            };

        return c;
    }

    public virtual SkillStat Subtract(SkillStat other)
    {
        SkillStat c;

        if (other == null)
            c = new SkillStat
            {
                baseCd = baseCd,
                maxStack = maxStack,
                duration = duration,
                dmg = dmg,
                stackGain = stackGain,
                baseCdRatio = baseCdRatio,
                maxStackRatio = maxStackRatio,
                durationRatio = durationRatio,
                dmgRatio = dmgRatio,
                Ratio = Ratio
            };
        else
            c = new SkillStat
            {
                baseCd = baseCd - other.baseCd,
                maxStack = maxStack - other.maxStack,
                duration = duration - other.duration,
                dmg = dmg - other.dmg,
                stackGain = stackGain - other.stackGain,
                baseCdRatio = baseCdRatio - other.baseCdRatio,
                maxStackRatio = maxStackRatio - other.maxStackRatio,
                durationRatio = durationRatio - other.durationRatio,
                dmgRatio = dmgRatio - other.dmgRatio,
                Ratio = Ratio - other.Ratio
            };

        return c;
    }

    public static SkillStat operator +(SkillStat a, SkillStat b)
    {
        return a.Combine(b);
    }

    public static SkillStat operator -(SkillStat a, SkillStat b)
    {
        return a.Subtract(b);
    }
}