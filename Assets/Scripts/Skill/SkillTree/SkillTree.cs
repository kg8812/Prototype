using Apis.Managers;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.SkillTrees
{
    public abstract class SkillTree : SerializedScriptableObject, ISkillVisitor
    {
        [SerializeField] [LabelText("인덱스")] private int index;

        protected int level;
        public int Level => level;

        public string Name { get; private set; }

        public string Description { get; private set; }

        public int Index => index;
        public PlayerType PlayerType { get; private set; }


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
                Name = LanguageManager.Str(data.name);
                Description = LanguageManager.Str(data.description);
                PlayerType = data.playerType;

                level = 0;
            }
        }
    }
}