using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyCheck 
{
    public bool CheckDestroyable(EventParameters parameters);
}

public class GroggyDestroyCheck : IDestroyCheck
{
    private readonly float groggyRequired;
    public GroggyDestroyCheck(float groggyRequired)
    {
        this.groggyRequired = groggyRequired;
    }
    public bool CheckDestroyable(EventParameters parameters)
    {
        return parameters.atkData.groggyAmount >= groggyRequired;
    }
}

public class AttackTypeCheck : IDestroyCheck
{
    private readonly Define.AttackType _attackType;
    
    public AttackTypeCheck(Define.AttackType attackType)
    {
        _attackType = attackType;
    }
    public bool CheckDestroyable(EventParameters parameters)
    {
        return parameters?.atkData != null && parameters.atkData.attackType == _attackType;
    }
}
