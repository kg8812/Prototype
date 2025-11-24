using Apis.DataType;
using System.Collections.Generic;
using System.Linq;

namespace Apis
{
    public class BuffDatabase : Database
    {
        public class Datas
        {
            public bool TryGetBuff(int buffIndex, out Buff buff)
            {
                if (buffDict.TryGetValue(buffIndex, out var value))
                {
                    buff = new Buff(value,null);
                    return true;
                }

                buff = null;
                return false;
            }

            public bool TryGetBuffList(int groupIndex, out List<BuffGroupDataType> buffGroup)
            {
                return buffGroupDict.TryGetValue(groupIndex, out buffGroup);
            }

            public bool TryGetSubBuffOption(int subBuffOptionindex, out SubBuffOptionDataType subBuffOption)
            {
                return subBuffDict.TryGetValue(subBuffOptionindex, out subBuffOption);
            }

            public bool TryGetSubBuffOption(SubBuffType type, out SubBuffOptionDataType subBuffOption)
            {
                TryGetSubBuffIndex(type, out int index);
                return subBuffDict.TryGetValue(index, out subBuffOption);
            }
            public bool TryGetSubBuffIndex(SubBuffType type, out int id)
            {
                return subBuffTypeDict.TryGetValue(type, out id);
            }

            public bool TryGetSubBuffType(int id, out SubBuffType type)
            {
                return subBuffTypeDict2.TryGetValue(id, out type);
            }

            public bool TryGetBuffGroupData(int groupId, out BuffGroupDataType data)
            {
                return buffGroupDataDict.TryGetValue(groupId, out data);
            }
        }

        static Dictionary<int, BuffDataType> buffDict;
        static Dictionary<int, List<BuffGroupDataType>> buffGroupDict;
        static Dictionary<int, BuffGroupDataType> buffGroupDataDict;
        static Dictionary<int, SubBuffOptionDataType> subBuffDict;
        static Dictionary<SubBuffType, int> subBuffTypeDict;
        static Dictionary<int, SubBuffType> subBuffTypeDict2;

        private static Datas datas;

        public static Datas DataLoad
        {
            get
            {
                if (GameManager.Data == null) return null;

                return datas ??= new();
            }
        }
        public override void ProcessDataLoad()
        {
            buffDict = GameManager.Data.GetDataTable<BuffDataType>(Apis.DataTableType.Buff).ToDictionary(
                x => int.Parse(x.Key), x =>
                {
                    x.Value.index = int.Parse(x.Key);
                    return x.Value;
                });
            subBuffDict = GameManager.Data.GetDataTable<SubBuffOptionDataType>(Apis.DataTableType.SubBuffOption)
                .ToDictionary(
                    x => int.Parse(x.Key), x => x.Value);

            {
                var dict = GameManager.Data.GetDataTable<BuffGroupDataType>(Apis.DataTableType.BuffGroup);
                buffGroupDict = dict.GroupBy(kv => kv.Value.buffGroup, kv => kv.Value)
                    .ToDictionary(kv => kv.Key, kv => kv.ToList());
                buffGroupDataDict = dict.GroupBy(kv => kv.Value.buffGroup)
                    .ToDictionary(group => group.Key, group => group.First().Value);
            }

            {
                var dict = GameManager.Data.GetDataTable<SubBuffTypeDataType>(Apis.DataTableType.SubBuffType);

                subBuffTypeDict = dict.ToDictionary(x => x.Value.typeName, x => int.Parse(x.Key));
                subBuffTypeDict2 = dict.ToDictionary(x => int.Parse(x.Key), x => x.Value.typeName);
            }
        }
    }
}