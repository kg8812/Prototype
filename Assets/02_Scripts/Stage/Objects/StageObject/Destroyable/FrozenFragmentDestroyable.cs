using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenFragmentDestroyable : FragmentDestroyable
{
    private AttackTypeCheck _destroyCheck;

    protected override IDestroyCheck DestroyCheck => _destroyCheck ??= new AttackTypeCheck(Define.AttackType.FrozenDrillAttack);

    protected override void DestroyObj(EventParameters parameters)
    {
        base.DestroyObj(parameters);
        
        
    }
}
