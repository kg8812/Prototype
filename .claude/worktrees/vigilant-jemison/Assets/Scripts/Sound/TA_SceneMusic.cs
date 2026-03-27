using System;
using System.Collections.Generic;
using Apis;

public class TA_SceneMusic : ITriggerActivate
{
    private readonly SceneBGMInfo _info;

    public TA_SceneMusic(SceneBGMInfo info)
    {
        _info = info;
    }

    public void Activate()
    {
        GameManager.Sound.PlaySceneBGM(_info);
    }

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
}