using System.Collections.Generic;
using System.Linq;
using Apis;
using Default;
using Unity.VisualScripting;
using UnityEngine;

namespace Apis
{
    public class Factory_ActiveSkillItem : ItemFactory<ActiveSkillItem>
    {
        readonly Dictionary<int, ActiveSkill> skillItemDict = new Dictionary<int, ActiveSkill>();
        public Dictionary<int, ActiveSkill> SkillItemDict => skillItemDict;

        private ActiveSkillItem skillItemPrefab;

        public Factory_ActiveSkillItem(ActiveSkill[] activeSkills, ActiveSkillItem[] skillItem): base(skillItem)
        {
            skillItemPrefab = skillItem[0];
            foreach (var x in activeSkills)
            {
                if(x.itemId != 0)
                    skillItemDict.TryAdd(x.itemId, x);
            }
        }
        public override ActiveSkillItem CreateNew(int itemId)
        {
            if (skillItemDict.TryGetValue(itemId, out var value))
            {
                ActiveSkillItem skillItem = pool.Get(skillItemPrefab.name);
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
            int rand = Random.Range(0, skillItemDict.Count);
            ActiveSkillItem skillItem = pool.Get(skillItemPrefab.name);
            skillItem.ActiveSkill = Object.Instantiate(skillItemDict.ElementAt(rand).Value);
            skillItem.ActiveSkill.Init();
            skillItem.Init();
            return skillItem;
        }

        public override List<ActiveSkillItem> CreateAll()
        {
            List<ActiveSkillItem> list = new();

            foreach (ActiveSkill skillItem in skillItemDict.Values)
            {
                ActiveSkillItem item = pool.Get(skillItemPrefab.name);
                item.ActiveSkill = Object.Instantiate(skillItem);
                item.ActiveSkill.Init();
                item.Init();
                list.Add(item);
            }

            return list;
        }
    }
}