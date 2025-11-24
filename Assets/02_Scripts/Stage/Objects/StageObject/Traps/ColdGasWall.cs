using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.StageObj;
using UnityEngine;

public class ColdGasWall : MonoBehaviour,TriggeredObj ,IAttackable
{
    [SerializeField] private GameObject wall;
    [SerializeField] private AttackObject coldGas;
    [SerializeField] private float dmg;

    private void Awake()
    {
        coldGas.Init(this,new AtkBase(this));
    }

    public void ChangeTrigger(int value)
    {
        wall.SetActive(value != 0);
        coldGas.gameObject.SetActive(value != 0);
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector3 TopPivot
    {
        get => Vector2.zero;
        set
        {
        }
    }

    public float Atk => dmg;

    public void AttackOn()
    {
    }

    public void AttackOff()
    {
    }

    public EventParameters Attack(EventParameters eventParameters)
    {
        if (eventParameters?.target == null || eventParameters.target.IsInvincible)
        {
            return null;
        }
        eventParameters.atkData.dmg = eventParameters.atkData.atkStrategy.Calculate(eventParameters.target);
            
        eventParameters.hitData.isCritApplied = false;

        eventParameters.hitData.dmg = eventParameters.atkData.dmg;

        eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
        return eventParameters;
    }
}
