using System.Collections;
using System.Collections.Generic;
using Apis;
using EventData;
using UnityEngine;

public class AttackTrap : MonoBehaviour , IAttackable
{
    [SerializeField] private GameObject child;
    [SerializeField] private float dmg;
    public float Atk => dmg;

    void Awake()
    {
        if (child == null)
            child = transform.GetChild(0).gameObject;
        if (child.TryGetComponent<AttackObject>(out var atkObj))
        {
            atkObj.Init(this, new AtkBase(this));
        }
    }
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
            
        // attacker가 없기 때문에 atkobj relative로 계산.
        if (eventParameters.knockBackData.directionType == KnockBackData.DirectionType.AttackerRelative)
        {
            eventParameters.knockBackData.directionType = KnockBackData.DirectionType.AktObjRelative;
        }
        if (eventParameters.groggyKnockBackData.directionType == KnockBackData.DirectionType.AttackerRelative)
        {
            eventParameters.groggyKnockBackData.directionType = KnockBackData.DirectionType.AktObjRelative;
        }
        
        eventParameters.hitData.dmg = eventParameters.atkData.dmg;

        eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
        return eventParameters;
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
}
