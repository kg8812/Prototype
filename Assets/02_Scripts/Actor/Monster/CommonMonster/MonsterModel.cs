using System.Collections.Generic;
using System.Linq;
using Apis.DataType;

namespace Apis
{
    public class MonsterModel : Database
    {
        public static Dictionary<int, MonsterDataType> monsterDict;

        public override void ProcessDataLoad()
        {
            monsterDict = GameManager.Data.GetDataTable<MonsterDataType>(DataTableType.Monster)
                .ToDictionary(x => int.Parse(x.Key), x => x.Value);
        }
    }
}