using Apis;
using UnityEngine;

public class TA_SetGameObject : ITriggerActivate
{
    private readonly bool isOn;
    private readonly GameObject obj;

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