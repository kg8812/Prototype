using Apis;

public class TA_SceneMusicFading : ITriggerActivate
{
    private readonly float fadeTime;

    private readonly int number;

    public TA_SceneMusicFading(int number, float fadeTime)
    {
        this.number = number;
        this.fadeTime = fadeTime;
    }

    public void Activate()
    {
        GameManager.Sound.SwapSceneBGMWithIndex(number, fadeTime);
    }
}