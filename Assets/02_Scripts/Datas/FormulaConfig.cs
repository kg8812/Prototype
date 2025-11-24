using Apis;
using Apis.DataType;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormulaConfig : Database
{
    public static float baseGold;
    public static float defConstant = 100;
    public static float basePotioncount;
    public static float cdConstant = 70;
    public static float groggyRecoverDelay = 3;
    public static float commonGroggyDuration;
    public static float eliteGroggyDuration;
    public static float bossGroggyDuration;
    public static float minMonsterRecognitionTime = 2;
    public static float minimumKnockBackForce = 1f;
    public static float minimumKnockBackTime = 0.5f;
    public static float shopDiscount = 0;
    
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

        baseGold = configDict[1002];
        basePotioncount = configDict[1003];
        defConstant = configDict[2001];
        cdConstant = configDict[2002];
        commonGroggyDuration = configDict[3001];
        eliteGroggyDuration = configDict[3002];
        bossGroggyDuration = configDict[3003];
        groggyRecoverDelay = configDict[3004];
    }
}
