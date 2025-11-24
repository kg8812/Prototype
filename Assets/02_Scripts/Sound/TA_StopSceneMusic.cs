using System.Collections;
using System.Collections.Generic;
using Apis;
using UnityEngine;

public class TA_StopSceneMusic : ITriggerActivate
{
    public void Activate()
    {
        GameManager.Sound.StopSceneBGM();
    }
}
