using System.Collections;
using System.Collections.Generic;
using Apis;
using UnityEngine;

public class TA_SetGameObject : ITriggerActivate
{
    private bool isOn;
    private GameObject obj;

    public TA_SetGameObject(GameObject obj, bool isOn)
    {
        this.isOn = isOn;
        this.obj = obj;
    }
    public void Activate()
    {
        obj.SetActive(isOn);
    }
}
