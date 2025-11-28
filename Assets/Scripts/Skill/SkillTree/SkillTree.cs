using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.SkillTrees
{
    public abstract class SkillTree : SerializedScriptableObject, ISkillVisitor
    {
        public enum SlotTypeEnum
        {
            Low,
            Medium,
            High
        }

        public enum TreeTypeEnum
        {
            Active,
            Passive,
            Support
        }

        [SerializeField] [LabelText("인덱스")] private int index;

        protected int level;
        public int Level => level;
        public SlotTypeEnum SlotType { get; private set; }

        public string Name { get; }

        public string Description { get; }

        public int Index => index;
        public PlayerType PlayerType { get; private set; }

        public TreeTypeEnum TreeType { get; private set; }

        public int[] TagNames { get; private set; }

        // 호출은 액티브 -> 패시브 순으로 호출됨.

        public virtual void Activate(PlayerActiveSkill active, int level)
        {
            this.level = level;
        }

        public virtual void Activate(PlayerPassiveSkill passive, int level)
        {
            this.level = level;
        }

        public virtual void DeActivate()
        {
            level = 0;
        }

        public virtual void Init()
        {
            if (SkillTreeDatas.TryGetSkillTreeData(index, out var data))
            {
                // _name = LanguageManager.Str(data.name);
                // description = LanguageManager.Str(data.description);
                PlayerType = data.playerType;
                TreeType = data.treeType;
                TagNames = data.tagNames;
                SlotType = data.slotType;

                level = 0;
            }
        }
    }
}