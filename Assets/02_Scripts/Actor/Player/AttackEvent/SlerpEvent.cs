using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackEvent(Slerp)", menuName = "AttackEvent/Slerp")]
public class SlerpEvent : AttackEvent
{
    public override void Invoke(Player p)
    {
        Debug.Log("Slerp");
    }
}
