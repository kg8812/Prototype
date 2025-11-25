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
        
        public SkillTree.TreeTypeEnum treeType;
        public SkillTree.SlotTypeEnum slotType;
        public int name;
        public int description;
        public int[] tagNames;
        
    }
}
