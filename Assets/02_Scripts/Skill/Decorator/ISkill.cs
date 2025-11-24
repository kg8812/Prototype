public interface ISkill
{
    public SkillStat Stat { get; }
}

public class SkillStat
{
    public float baseCd;
    public int maxStack;
    public float duration;
    public float dmg;
    public float groggy;
    public int stackGain;
    
    public float baseCdRatio;
    public float maxStackRatio;
    public float durationRatio;
    public float dmgRatio;
    public float groggyRatio;

    public SkillStat()
    {
    }

    public virtual void Reset()
    {
        baseCd = 0;
        maxStack = 0;
        duration = 0;
        dmg = 0;
        groggy = 0;
        baseCdRatio = 0;
        maxStackRatio = 0;
        durationRatio = 0;
        dmgRatio = 0;
        groggyRatio = 0;
        stackGain = 0;
    }

    public virtual SkillStat Combine(SkillStat other)
    {
        SkillStat c;

        if (other == null)
        {
            c = new()
            {
                baseCd = baseCd,
                maxStack = maxStack,
                duration = duration,
                dmg = dmg,
                groggy = groggy,
                stackGain = stackGain,
                baseCdRatio = baseCdRatio,
                maxStackRatio = maxStackRatio,
                durationRatio = durationRatio,
                dmgRatio = dmgRatio,
                groggyRatio = groggyRatio,
            };
        }
        else
        {
            c = new()
            {
                baseCd = baseCd + other.baseCd,
                maxStack = maxStack + other.maxStack,
                duration = duration + other.duration,
                dmg = dmg + other.dmg,
                groggy = groggy + other.groggy,
                stackGain = stackGain +other.stackGain,
                baseCdRatio = baseCdRatio + other.baseCdRatio,
                maxStackRatio = maxStackRatio + other.maxStackRatio,
                durationRatio = durationRatio + other.durationRatio,
                dmgRatio = dmgRatio + other.dmgRatio,
                groggyRatio = groggyRatio + other.groggyRatio,
            };
        }
        
        return c;
    }

    public virtual SkillStat Subtract(SkillStat other)
    {
        SkillStat c;

        if (other == null)
        {
            c = new()
            {
                baseCd = baseCd,
                maxStack = maxStack,
                duration = duration,
                dmg = dmg,
                groggy = groggy,
                stackGain = stackGain,
                baseCdRatio = baseCdRatio,
                maxStackRatio = maxStackRatio,
                durationRatio = durationRatio,
                dmgRatio = dmgRatio,
                groggyRatio = groggyRatio,
            };
        }
        else
        {
            c = new()
            {
                baseCd = baseCd - other.baseCd,
                maxStack = maxStack - other.maxStack,
                duration = duration - other.duration,
                dmg = dmg - other.dmg,
                groggy = groggy - other.groggy,
                stackGain = stackGain -other.stackGain,
                baseCdRatio = baseCdRatio - other.baseCdRatio,
                maxStackRatio = maxStackRatio - other.maxStackRatio,
                durationRatio = durationRatio - other.durationRatio,
                dmgRatio = dmgRatio - other.dmgRatio,
                groggyRatio = groggyRatio - other.groggyRatio,
            };
        }

        return c;
    }

    public SkillStat(float baseCd, int maxStack, float duration, float dmg, float groggy)
    {
        this.baseCd = baseCd;
        this.maxStack = maxStack;
        this.duration = duration;
        this.dmg = dmg;
        this.groggy = groggy;
        baseCdRatio = 0;
        maxStackRatio = 0;
        durationRatio = 0;
        dmgRatio = 0;
        groggyRatio = 0;
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