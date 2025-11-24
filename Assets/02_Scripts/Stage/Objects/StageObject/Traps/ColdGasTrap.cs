using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

public class ColdGasTrap : MonoBehaviour , IAttackable
{
    [LabelText("데미지")] public float dmg;
    [LabelText("활성화시간")] public float activeDuration;
    [LabelText("비활성화시간")] public float deActiveDuration;

    [SerializeField] private AttackObject coldGas;
    private void Awake()
    {
        StartCoroutine(Deactivate());
    }

    IEnumerator Activate()
    {
        coldGas.gameObject.SetActive(true);
        coldGas.Init(this,new AtkBase(this));
        yield return new WaitForSeconds(activeDuration);
        StartCoroutine(Deactivate());
    }

    IEnumerator Deactivate()
    {
        coldGas.gameObject.SetActive(false);
        yield return new WaitForSeconds(deActiveDuration);
        StartCoroutine(Activate());
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