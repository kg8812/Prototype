namespace Apis.SkillTrees
{
    // 고유트리 방랑자 인터페이스
    public interface ISkillVisitor
    {
        
        public void Activate(PlayerActiveSkill active, int level);// 액티브 스킬 고유트리 적용
        public void Activate(PlayerPassiveSkill passive, int level); // 패시브 스킬 고유트리 적용
        public void DeActivate(); // 고유트리 해제
    }
}