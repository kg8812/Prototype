using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackEventContainer", menuName = "Scriptable/AttackEventContainer")]
public class AttackEventContainer : ScriptableObject
{
    public List<AttackEvent> AttackEvents;
}
