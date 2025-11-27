using System;
using System.Collections.Generic;
using System.Linq;
using Default;
using UnityEngine;

namespace Apis
{
    public class SubBuffResources
    {
        private static List<List<string>> _buffList;
        private static bool isInit;
        private static List<List<string>> buffList => _buffList ??= new List<List<string>>();

        public static void Init()
        {
            if (isInit) return;

            var texts = ResourceUtil.LoadAll<TextAsset>("SubBuffTexts");
            texts = texts.OrderBy(x => int.Parse(x.name.Remove(0, 8))).ToArray();

            for (var i = 0; i < texts.Length; i++)
            {
                var textAsset = texts[i];

                var list = textAsset.text.Split(',').ToList();
                list = list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                for (var j = 0; j < list.Count; j++)
                {
                    list[j] = list[j].Replace(" ", "").Replace(",", "").Replace("\n", "").Replace("\t", "").Trim();

                    list[j] = "Apis." + list[j];
                }

                buffList.Add(list);
            }

            isInit = true;
        }

        public static SubBuff Get(Buff buff)
        {
            if (!isInit) Init();

            if (buff.BuffMainType >= buffList.Count)
            {
                Debug.Log("버프를 찾을 수 없음");

                return null;
            }

            if (buff.BuffSubType >= buffList[buff.BuffMainType].Count)
            {
                Debug.Log("버프를 찾을 수 없음");
                return null;
            }

            var tp = Type.GetType(buffList[buff.BuffMainType][buff.BuffSubType]);

            object[] arg = { buff };

            if (tp != null) return Activator.CreateInstance(tp, arg) as SubBuff;

            return null;
        }

        public static SubBuffType GetType(int mainType, int subType)
        {
            foreach (var x in Utils.SubBuffTypes)
                if (x.ToString().Equals(buffList[mainType][subType]))
                    return (SubBuffType)Enum.Parse(typeof(SubBuffType), x.ToString());

            return SubBuffType.None;
        }
    }
}