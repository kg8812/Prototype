using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apis
{
    public class Factory_ActiveSkillItem : ItemFactory<ActiveSkillItem>
    {
        private readonly ActiveSkillItem skillItemPrefab;

        public Factory_ActiveSkillItem(ActiveSkill[] activeSkills, ActiveSkillItem[] skillItem) : base(skillItem)
        {
            skillItemPrefab = skillItem[0];
            foreach (var x in activeSkills)
                if (x.itemId != 0)
                    SkillItemDict.TryAdd(x.itemId, x);
        }

        public Dictionary<int, ActiveSkill> SkillItemDict { get; } = new();

        public override ActiveSkillItem CreateNew(int itemId)
        {
            if (SkillItemDict.TryGetValue(itemId, out var value))
            {
                var skillItem = pool.Get(skillItemPrefab.name);
                // TODO 새로 생성하는건지 아니면 그냥 할당인지.
                skillItem.ActiveSkill = Object.Instantiate(value);
                skillItem.ActiveSkill.Item = skillItem;
                skillItem.ActiveSkill.Init();
                skillItem.Init();
                return skillItem;
            }

            return null;
        }

        public override ActiveSkillItem CreateRandom()
        {
            var rand = Random.Range(0, SkillItemDict.Count);
            var skillItem = pool.Get(skillItemPrefab.name);
            skillItem.ActiveSkill = Object.Instantiate(SkillItemDict.ElementAt(rand).Value);
            skillItem.ActiveSkill.Init();
            skillItem.Init();
            return skillItem;
        }

        public override List<ActiveSkillItem> CreateAll()
        {
            List<ActiveSkillItem> list = new();

            foreach (var skillItem in SkillItemDict.Values)
            {
                var item = pool.Get(skillItemPrefab.name);
                item.ActiveSkill = Object.Instantiate(skillItem);
                item.ActiveSkill.Init();
                item.Init();
                list.Add(item);
            }

            return list;
        }
    }
}