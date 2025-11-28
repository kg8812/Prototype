using System.Collections.Generic;
using System.Linq;

namespace Apis.DataType
{
    public class LevelDatabase : Database
    {
        private static Dictionary<int, LevelDataType> levelDict;

        public override void ProcessDataLoad()
        {
            levelDict = GameManager.Data.GetDataTable<LevelDataType>(DataTableType.Level)
                .ToDictionary(x => x.Value.index, x => x.Value);
        }

        public static LevelDataType GetLevelData(int level)
        {
            return levelDict.GetValueOrDefault(level);
        }
    }
}