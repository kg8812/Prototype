using System;
using Save.Schema;
using Apis.SkillTrees;

namespace Apis.DataType
{
    [Serializable]
    public class SkillTreeDataType
    {
        public int index;
        public PlayerType playerType;
        public TagManager.SkillTreeTag[] tags;
        
        public SkillTree.TreeTypeEnum treeType;
        public SkillTree.SlotTypeEnum slotType;
        public int name;
        public int description;
        public int[] tagNames;
        
    }
}
