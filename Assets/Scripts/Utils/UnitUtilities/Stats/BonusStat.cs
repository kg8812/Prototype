using System;
using System.Collections.Generic;
using Apis;
using Default;

[Serializable]
public class BonusStat
{
    // 영구 추가 스탯 관련 클래스 (무기, 악세서리 등)

    public Dictionary<ActorStatType, IStat> Stats;

    public BonusStat()
    {
        Stats = new Dictionary<ActorStatType, IStat>(Utils.StatTypes.Length);

        foreach (var x in Utils.StatTypes) Stats.Add(x, new BasicStat(x));
    }

    public BonusStat(BonusStat other)
    {
        Stats = new Dictionary<ActorStatType, IStat>(Utils.StatTypes.Length);
        foreach (var x in Utils.StatTypes)
        {
            Stats.Add(x, new BasicStat(x));
            Stats[x].Value = other.Stats[x].Value;
            Stats[x].Ratio = other.Stats[x].Ratio;
        }
    }

    public void Reset() // 초기화 함수
    {
        foreach (var x in Utils.StatTypes)
            if (Stats.ContainsKey(x))
            {
                Stats[x].Value = 0;
                Stats[x].Ratio = 0;
            }
    }

    public void AddValue(ActorStatType type, float value) // 값 추가
    {
        Stats[type].Value += value;
    }

    public void AddRatio(ActorStatType type, float ratio) // 배율 추가
    {
        Stats[type].Ratio += ratio;
    }

    public void SetValue(ActorStatType type, float value)
    {
        Stats[type].Value = value;
    }

    public void SetRatio(ActorStatType type, float ratio)
    {
        Stats[type].Ratio = ratio;
    }

    public static BonusStat operator +(BonusStat a, BonusStat b)
    {
        BonusStat c = new();

        foreach (var x in Utils.StatTypes)
        {
            c.Stats[x].Value = a.Stats[x].Value + b.Stats[x].Value;
            c.Stats[x].Ratio = a.Stats[x].Ratio + b.Stats[x].Ratio;
        }

        return c;
    }

    public static BonusStat operator -(BonusStat a, BonusStat b)
    {
        BonusStat c = new();
        foreach (var x in Utils.StatTypes)
        {
            c.Stats[x].Value = a.Stats[x].Value - b.Stats[x].Value;
            c.Stats[x].Ratio = a.Stats[x].Ratio - b.Stats[x].Ratio;
        }

        return c;
    }
}