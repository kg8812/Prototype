using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.DataType;
using UnityEngine;

public class FormulaConfig : Database
{
    public const float uiFadeInDuration = 0.8f;
    public static float defConstant = 100;
    public static float cdConstant = 70;
    public static float RecoverDelay = 3;
    public static float commonDuration;
    public static float eliteDuration;
    public static float bossDuration;

    private Dictionary<int, float> configDict = new();

    public static float CalculateCD(float cdReduction, float cd)
    {
        float value = Mathf.RoundToInt((1 - cdConstant / (cdConstant + cdReduction)) * 100);
        return cd * (1 - value / 100);
    }

    public override void ProcessDataLoad()
    {
        configDict = GameManager.Data.GetDataTable<ConfigDataType>(DataTableType.Config)
            .ToDictionary(kv => int.Parse(kv.Key), kv => kv.Value.number);

        //테이블 바탕으로 초기 데이터 설정
    }
}