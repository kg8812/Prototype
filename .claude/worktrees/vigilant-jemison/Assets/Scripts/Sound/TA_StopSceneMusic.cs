using Apis;

public class TA_StopSceneMusic : ITriggerActivate
{
    public void Activate()
    {
        GameManager.Sound.StopSceneBGM();
    }
}