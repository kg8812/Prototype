using System;
using Apis.SkillTrees;

namespace Apis.DataType
{
    [Serializable]
    public class SkillTreeDataType
    {
        public int index;
        public PlayerType playerType;

        public int name;
        public int description;
    }
}