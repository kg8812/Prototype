using Apis;

public class SkillConfig : ISkill
{
    public int SkillName;
    public int Desc;
    
    public SkillConfig(SkillStat stat)
    {
        Stat = stat;
    }

    public SkillStat Stat { get; }
}

public class SkillAttachment : ISkill
{
    public virtual SkillStat Stat { get; }

    public SkillAttachment(SkillStat stat)
    {
        Stat = stat;
    }
}

public class SkillDecorator : ISkill
{
    public SkillStat Stat
    {
        get
        {
            return config.Stat + attachment.Stat;
        }
    }

    private readonly ISkill config;
    private readonly ISkill attachment;

    public SkillDecorator(ISkill skill, ISkill attachment)
    {
        config = skill;
        this.attachment = attachment;
    }
}

public class PlayerSkillAttachment : ISkill
{

    private SkillStat _stat;

    public SkillStat Stat
    {
        get
        {
            _stat ??= new();

            return _stat;
        }
    }
}
