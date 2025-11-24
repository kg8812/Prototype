using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis;
using Apis.Components;
using UnityEngine;

public class TA_MonsterLab : ITriggerActivate
{
    private readonly Trigger trigger;
    readonly List<int> spawnInfos;
    private readonly string effectAddress;
    public TA_MonsterLab(Trigger trigger,List<int> spawnInfos,string effectAddress)
    {
        this.spawnInfos = spawnInfos;
        this.trigger = trigger;
        this.effectAddress = effectAddress;
    }
    public void Activate()
    {
        if (!string.IsNullOrEmpty(effectAddress))
        {
            GameManager.Factory.Get(FactoryManager.FactoryType.Effect, effectAddress, trigger.transform.position);
        }
        spawnInfos.ForEach(x =>
        {
            var monster = GameManager.Factory.Get<Monster>(FactoryManager.FactoryType.Monster, x.ToString(), trigger.transform.position);
            monster.MoveToFloor();
        });
    }
}
