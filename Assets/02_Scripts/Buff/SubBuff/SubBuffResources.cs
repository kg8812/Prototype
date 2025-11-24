using System;
using System.Collections.Generic;
using UnityEngine;
using Default;
using System.Linq;

namespace Apis
{
    public class SubBuffResources
    {
        static List<List<string>> _buffList;
        private static List<List<string>> buffList => _buffList ??= new();
        static bool isInit;
        
        public static void Init()
        {
            if (isInit) return;

            TextAsset[] texts = ResourceUtil.LoadAll<TextAsset>("SubBuffTexts");
            texts = texts.OrderBy(x => int.Parse(x.name.Remove(0,8))).ToArray();

            for (int i = 0; i < texts.Length; i++)
            {
                TextAsset textAsset = texts[i];

                List<string> list = textAsset.text.Split(',').ToList();
                list = list.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

                for (int j = 0; j < list.Count; j++)
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
            
            Type tp = Type.GetType(buffList[buff.BuffMainType][buff.BuffSubType]);
            
            object[] arg = { buff };

            if (tp != null) return Activator.CreateInstance(tp, arg) as SubBuff;
            
            return null;

        }        

        public static SubBuffType GetType(int mainType, int subType)
        {
            foreach (var x in Utils.SubBuffTypes)
            {
                if (x.ToString().Equals(buffList[mainType][subType]))
                {
                    return (SubBuffType)Enum.Parse(typeof(SubBuffType), x.ToString());
                }
            }

            return SubBuffType.None;
        }
    }
}
