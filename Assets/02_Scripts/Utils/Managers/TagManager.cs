using System;
using System.Collections.Generic;
using Default;
using UnityEngine;

public class TagManager
{
    public enum SkillTreeTag
    {
        Feather,Spell,Depiction,Coloring,Rifle,
        Hunt,Ghost,Soldier,Drone,Mecha,Shield,Beast,
        None
    }

    private TagData _tagData;

    public TagData Data => _tagData;
    
    private readonly Dictionary<SkillTreeTag, int> skillTreeTags;

    public TagManager()
    {
        skillTreeTags = new Dictionary<SkillTreeTag, int>();
        foreach (SkillTreeTag tag in Enum.GetValues(typeof(SkillTreeTag)))
        {
            skillTreeTags.Add(tag,0);
        }

        _tagData = ResourceUtil.Load<TagData>("TagData");
    }

    public void AddTag(SkillTreeTag tag)
    {
        skillTreeTags[tag]++;
    }

    public void MinusTag(SkillTreeTag tag)
    {
        if (skillTreeTags[tag] > 0)
        {
            skillTreeTags[tag]--;
        }
    }
    public int GetTagCount(SkillTreeTag tag)
    {
        return skillTreeTags[tag];
    }
}
