using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.DataType;

public class LobbyDatabase : Database
{
    public static Dictionary<int, ConvenienceDataType> convenienceDict = new();
    public static Dictionary<ActorStatType, Dictionary<int, GrowthDataType>> growthDict = new();

    public override void ProcessDataLoad()
    {
        convenienceDict = GameManager.Data.GetDataTable<ConvenienceDataType>(DataTableType.Convenience)
            .ToDictionary(kv => int.Parse(kv.Key), kv => kv.Value);

        growthDict = GameManager.Data.GetDataTable<GrowthDataType>(DataTableType.Growth).GroupBy(
            kv => kv.Value.statGroup, kv => kv.Value
        ).ToDictionary(kv => kv.Key,
            kv => kv.ToDictionary(x => x.lvl, x => x));
    }


    public static bool TryGetConvenienceData(int index, out ConvenienceDataType data)
    {
        return convenienceDict.TryGetValue(index, out data);
    }

    public static bool TryGetGrowthData(ActorStatType statType, out Dictionary<int, GrowthDataType> data)
    {
        return growthDict.TryGetValue(statType, out data);
    }
}