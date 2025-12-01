using System.Collections.Generic;

namespace Apis
{
    public interface IActiveSkillUser : IMonoBehaviour
    {
        // 액티브 스킬 사용자
        public ActiveSkill curSkill { get; set; }
        public ActiveSkill ActiveSkill { get; }
        public List<SkillAttachment> ActiveAttachments { get; }

        public void AddActiveSkillAttachment(SkillAttachment attachment)
        {
            ActiveAttachments.Add(attachment);
            ActiveSkill?.Decorate();
        }

        public void RemoveActiveSkillAttachment(SkillAttachment attachment)
        {
            ActiveAttachments.Remove(attachment);
            ActiveSkill?.Decorate();
        }
    }

    public interface IPassiveSkillUser : IMonoBehaviour
    {
        // 패시브 스킬 사용자
        public PassiveSkill PassiveSkill { get; }
    }
    
    public class SkillEventData : IEventData
    {
        public Skill usedSkill; // 사용한 스킬

        public void Reset()
        {
            usedSkill = null;
        }
    }
}