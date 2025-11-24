using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Default;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

public class SFXPlayer : MonoBehaviour,IPoolObject
{
    public AudioSourceUtil.PlayingType playingType;
    public string audioSettingAddress;

    public string[] audioClipsAddress;

    public float delay;
    public bool isDestroy;
    private AudioSourceUtil _audioUtil;
    public bool isLoop;
    private Sequence seq;
    [SerializeField] bool playWhenSpawn = true;

    [HideInInspector] public IMonoBehaviour user;
        
    public Define.Sound soundType = Define.Sound.SFX;
    
    public void Init(IMonoBehaviour user)
    {
        this.user = user;
    }
    public void Play()
    {
        _audioUtil = GameManager.Sound.PlayInPosition(audioClipsAddress,audioSettingAddress,playingType,user?.Position ?? transform.position,isLoop,soundType);
    }
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
        {
            _audioUtil?.Destroy();
        }
        else
        {
            _audioUtil?.Stop();
        }
    }
}
