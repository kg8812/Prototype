using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

public class TA_SceneMusic : ITriggerActivate
{
    [Serializable]
    public struct SceneBGMInfo
    {
        public List<string> clipAddresses;
        public int initialNumber;
        public float fadeOutTime;
        public float delay;
        public float fadeInTime;
        public int channel;
    }
    SceneBGMInfo _info;

    public TA_SceneMusic(SceneBGMInfo info)
    {
        _info = info;
    }

    public void Activate()
    {
        GameManager.Sound.PlaySceneBGM(_info);

    }
}
