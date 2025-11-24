using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.DataType;
using Default;
using Save.Schema;
using Sirenix.Utilities;
using UnityEngine;

namespace Apis.SkillTrees
{
    public class SkillTreeDatas : Database
    {
        static Dictionary<int, SkillTree> skillTrees;

        static Dictionary<int, SkillTreeDataType> dataDict;

        private static HashSet<int> _equippedIndex = new();
        public static HashSet<int> equippedIndex => _equippedIndex ??= new();

        private static HashSet<int> _activatedIndex = new();
        public static HashSet<int> activatedIndex => _activatedIndex ??= new();
        
        public static void ApplySkillTree(int index,int level)
        {
            if (skillTrees.TryGetValue(index, out var tree))
            {
                if (tree.PlayerType != GameManager.instance.Player.playerType) return;

                if (!equippedIndex.Contains(index))
                {
                    foreach (var skillTreeTag in tree.Tags)
                    {
                        GameManager.Tag.AddTag(skillTreeTag);
                    }
                }

                if (GameManager.instance.Player.ActiveSkill is PlayerActiveSkill active)
                {
                    active.Accept(tree,level);
                }
                if (GameManager.instance.Player.PassiveSkill is PlayerPassiveSkill passive)
                {
                    passive.Accept(tree,level);
                }

                equippedIndex.Add(index);
            }
        }

        public static void DeApplySkillTree(int index)
        {
            if (skillTrees.TryGetValue(index, out var tree) && equippedIndex.Contains(index))
            {
                if (equippedIndex.Contains(index))
                {
                    foreach (var skillTreeTag in tree.Tags)
                    {
                        GameManager.Tag.MinusTag(skillTreeTag);
                    }
                }

                tree.DeActivate();

                equippedIndex.Remove(index);
            }
        }

        public static void DeActiveAll()
        {
            var list = equippedIndex.ToList();
            list.ForEach(DeApplySkillTree);
        }

        public static bool TryGetSkillTreeData(int index, out SkillTreeDataType data)
        {
            return dataDict.TryGetValue(index, out data);
        }

        public override void ProcessDataLoad()
        {
            dataDict = GameManager.Data.GetDataTable<SkillTreeDataType>(DataTableType.SkillTree)
                .ToDictionary(x => int.Parse(x.Key), x => x.Value);
            skillTrees = ResourceUtil.LoadAll<SkillTree>("SkillTrees").ToDictionary(x => x.Index, x => x);

            skillTrees.Values.ForEach(x => { x.Init(); });
        }

        public static List<SkillTree> GetAvailableSkillTrees()
        {
            return skillTrees.Values.Where(x =>
                    !equippedIndex.Contains(x.Index) && activatedIndex.Contains(x.Index) && GameManager.instance.Player.playerType == x.PlayerType)
                .OrderBy(x => x.Index).ToList();
        }

        public static List<SkillTree> GetActivatedSkillTrees()
        {
            return equippedIndex.Select(x => skillTrees[x]).OrderBy(x => x.Index).ToList();
        }


        public static List<SkillTree> GetSkillTreeList(PlayerType playerType)
        {
            return skillTrees.Values.Where(x => x.PlayerType == playerType).OrderBy(x => x.Index).ToList();
        }

        public static List<SkillTree> GetSkillTreeList()
        {
            return skillTrees.Values.OrderBy(x => x.Index).ToList();
        }

        public static SkillTree GetSkillTree(int index)
        {
            return skillTrees[index];
        }
    }
}