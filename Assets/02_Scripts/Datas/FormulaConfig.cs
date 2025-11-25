using Apis;
using Apis.DataType;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormulaConfig : Database
{
    public static float defConstant = 100;
    public static float cdConstant = 70;
    public static float groggyRecoverDelay = 3;
    public static float commonGroggyDuration;
    public static float eliteGroggyDuration;
    public static float bossGroggyDuration;
    
    public const float uiFadeInDuration = 0.8f;
    
    public static float CalculateCD(float cdReduction, float cd)
    {
        float value = Mathf.RoundToInt((1 - (cdConstant / (cdConstant + cdReduction))) * 100);
        return cd * (1 - value / 100);
    }

    Dictionary<int, float> configDict = new();
    public override void ProcessDataLoad()
    {
        configDict = GameManager.Data.GetDataTable<ConfigDataType>(DataTableType.Config).
            ToDictionary(kv => int.Parse(kv.Key), kv => kv.Value.number);

        //테이블 바탕으로 초기 데이터 설정
    }
}
