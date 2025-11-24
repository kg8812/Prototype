using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.DataType;

public class CutSceneDatabase : Database
{
    private static Dictionary<int, CutSceneDataType> dict;

    public override void ProcessDataLoad()
    {
        dict = GameManager.Data.GetDataTable<CutSceneDataType>(DataTableType.CutScene).ToDictionary(
            x => int.Parse(x.Key), x => x.Value);
    }

    public static bool TryGetCutSceneData(int id, out CutSceneDataType data)
    {
        return dict.TryGetValue(id, out data);
    }
}