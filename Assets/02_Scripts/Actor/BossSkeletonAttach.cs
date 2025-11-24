using System.Collections;
using System.Collections.Generic;
using Apis;
using UnityEngine;

public class BossSkeletonAttach : SkeletonAttach
{
    private BossMonster _boss;
    
    protected override void Awake()
    {
        base.Awake();
        _boss = Actor as BossMonster;
        ;
    }

    public void DoBossAttack(string value)
    {
        if (_boss != null)
        {
            _boss.DoAttack(value);
        }
    }


    public void SetColliders(string value)
    {
        if (_boss != null)
        {
            _boss.SetColliders(value);
        }
    }

    public void SetState(BossMonster.BossState state)
    {
        _boss?.SetState(state);
    }
    
    public void SpawnEffectInBone(string address)
    {
        _boss.EffectSpawner.Spawn(address, "center",true);
    }

    public void SpawnEffectInPosition(string address)
    {
        _boss.EffectSpawner.Spawn(address, _boss.Position,true);
    }

    public void SpawnEffectInPositionButNoParent(string address)
    {
        _boss.EffectSpawner.Spawn(address, _boss.Position,false);
    }
    public void ReturnEffect(string address)
    {
        _boss.EffectSpawner.Remove(address);
    }
}
