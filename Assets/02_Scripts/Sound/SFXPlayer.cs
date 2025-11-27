using DG.Tweening;
using UnityEngine;

public class SFXPlayer : MonoBehaviour, IPoolObject
{
    public AudioSourceUtil.PlayingType playingType;
    public string audioSettingAddress;

    public string[] audioClipsAddress;

    public float delay;
    public bool isDestroy;
    public bool isLoop;
    [SerializeField] private bool playWhenSpawn = true;

    public Define.Sound soundType = Define.Sound.SFX;
    private AudioSourceUtil _audioUtil;
    private Sequence seq;

    [HideInInspector] public IMonoBehaviour user;

    public void OnGet()
    {
        if (playWhenSpawn)
        {
            seq?.Kill();
            seq = DOTween.Sequence();
            seq.SetDelay(delay);
            seq.AppendCallback(Play);
        }
    }

    public void OnReturn()
    {
        if (isDestroy)
            _audioUtil?.Destroy();
        else
            _audioUtil?.Stop();
    }

    public void Init(IMonoBehaviour user)
    {
        this.user = user;
    }

    public void Play()
    {
        _audioUtil = GameManager.Sound.PlayInPosition(audioClipsAddress, audioSettingAddress, playingType,
            user?.Position ?? transform.position, isLoop, soundType);
    }
}