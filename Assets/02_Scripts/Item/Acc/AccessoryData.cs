using Apis.DataType;
using System.Collections.Generic;
using System.Linq;

public class AccessoryData : Database
{
    public class Datas
    {
        public bool TryGetData(int id, out AccessoryDataType data)
        {
            return dataDict.TryGetValue(id, out data);
        }

    }
    static Dictionary<int, AccessoryDataType> dataDict;

    static Datas datas;
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
        dataDict = GameManager.Data.GetDataTable<AccessoryDataType>(Apis.DataTableType.Accessory)
            .ToDictionary(x => int.Parse(x.Key), x => x.Value);
    }  
}
