using System.Collections;
using System.Collections.Generic;
using Apis;
using UnityEngine;

public class TA_SceneMusicFading : ITriggerActivate
{
    
    readonly int number;
    readonly float fadeTime;

    public TA_SceneMusicFading(int number, float fadeTime)
    {
        this.number = number;
        this.fadeTime = fadeTime;
    }
    public void Activate()
    {
        GameManager.Sound.SwapSceneBGMWithIndex(number,fadeTime);
    }
}
