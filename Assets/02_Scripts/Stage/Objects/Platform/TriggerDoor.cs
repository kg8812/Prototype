using System.Collections;
using System.Collections.Generic;
using Apis.StageObj;
using UnityEngine;

public class TriggerDoor : Door,TriggeredObj
{ 
    public void ChangeTrigger(int value)
    {
        MoveDoor(value == 1);
    }
}
