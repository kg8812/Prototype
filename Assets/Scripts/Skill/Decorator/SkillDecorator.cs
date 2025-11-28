public class SkillConfig : ISkill
{
    public int Desc;
    public int SkillName;

    public SkillConfig(SkillStat stat)
    {
        Stat = stat;
    }

    public SkillStat Stat { get; }
}

public class SkillAttachment : ISkill
{
    public SkillAttachment(SkillStat stat)
    {
        Stat = stat;
    }

    public virtual SkillStat Stat { get; }
}

public class SkillDecorator : ISkill
{
    private readonly ISkill attachment;

    private readonly ISkill config;

    public SkillDecorator(ISkill skill, ISkill attachment)
    {
        config = skill;
        this.attachment = attachment;
    }

    public SkillStat Stat => config.Stat + attachment.Stat;
}

public class PlayerSkillAttachment : ISkill
{
    private SkillStat _stat;

    public SkillStat Stat
    {
        get
        {
            _stat ??= new SkillStat();

            return _stat;
        }
    }
}