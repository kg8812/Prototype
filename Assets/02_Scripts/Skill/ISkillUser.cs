using System.Collections.Generic;
using UnityEngine;

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
        public PassiveSkill PassiveSkill { get;}
    }

    public interface IWeaponSkillUser : IMonoBehaviour
    {
        public MagicSkill MagicSkill { get; }
        public List<SkillAttachment> WeaponSkillAttachments { get; }

        public void AddWeaponSkillAttachment(SkillAttachment attachment)
        {
            WeaponSkillAttachments.Add(attachment);
            MagicSkill?.Decorate();
        }
        public void RemoveWeaponSkillAttachment(SkillAttachment attachment)
        {
            WeaponSkillAttachments.Remove(attachment);
            MagicSkill?.Decorate();
        }
    }
}