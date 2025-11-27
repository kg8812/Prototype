using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.DataType;

public class AccessoryData : Database
{
    private static Dictionary<int, AccessoryDataType> dataDict;

    private static Datas datas;

    public static Datas DataLoad
    {
        get
        {
            if (GameManager.Data == null) return null;
            return datas ??= new Datas();
        }
    }

    public override void ProcessDataLoad()
    {
        dataDict = GameManager.Data.GetDataTable<AccessoryDataType>(DataTableType.Accessory)
            .ToDictionary(x => int.Parse(x.Key), x => x.Value);
    }

    public class Datas
    {
        public bool TryGetData(int id, out AccessoryDataType data)
        {
            return dataDict.TryGetValue(id, out data);
        }
    }
}