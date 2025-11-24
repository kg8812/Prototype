using Apis.SkillTrees;

public interface IPlayerSkill
{
    // 고유트리용 방랑자 Acceptor
    public void Accept(ISkillVisitor visitor,int level);
}
