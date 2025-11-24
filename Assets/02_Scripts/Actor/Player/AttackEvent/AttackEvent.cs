using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackEvent : ScriptableObject
{
    public bool BlendLegAction = true;
    public abstract void Invoke(Player p);
}
