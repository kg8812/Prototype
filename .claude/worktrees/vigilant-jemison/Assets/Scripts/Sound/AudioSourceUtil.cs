using System;
using System.Collections;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceUtil : MonoBehaviour
{
    public enum PlayingType
    {
        Random, InOrder, IntroThenLoop
    }
    
    AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource ??= GetComponent<AudioSource>();

    UnityEvent _onLoop;
    UnityEvent _onEnd;
    public UnityEvent OnLoop => _onLoop ??= new();
    public UnityEvent OnEnd => _onEnd ??= new();

    Coroutine playCoroutine;

    private bool stopPlaying;

    private bool _isLoop;
    
    public void PlaySingle(AudioClip clip, bool isLoop = false)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        stopPlaying = false;
        _isLoop = isLoop;
        playCoroutine = StartCoroutine(PlaySingleCoroutine(clip));
    }

    IEnumerator PlaySingleCoroutine(AudioClip clip)
    {
        AudioSource.loop = _isLoop;

        yield return Play(clip);

        if (!_isLoop)
            OnEnd.Invoke();
    }
    
    public void Play(PlayingType playingType,CustomQueue<AudioClip> clips,bool isLoop)
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }

        AudioSource.Stop();
        AudioSource.loop = false;

        this._isLoop = isLoop;
        stopPlaying = false;
        switch (playingType)
        {
            case PlayingType.Random:
                playCoroutine = StartCoroutine(PlayInRandom(clips));
                break;
            case PlayingType.InOrder:
                playCoroutine = StartCoroutine(PlayInOrder(clips));
                break;
            case PlayingType.IntroThenLoop:
                playCoroutine = StartCoroutine(PlayIntroThenLoop(clips));
                break;
        }
    }
    
    IEnumerator Play(AudioClip clip)
    {
        AudioSource.Stop();
        if (AudioSource.clip != clip)
        {
            AudioSource.clip = clip;
        }

        AudioSource.Play();
        while (AudioSource.isPlaying)
        {
            yield return null;
        }
    }

    IEnumerator PlayRandom(CustomQueue<AudioClip> clips)
    {
        if (clips.Count == 0)
        {
            Debug.LogError("클립 개수 오류");
            yield break;
        }
        int rand = Random.Range(0, clips.Count);
        var clip = clips[rand];
        yield return Play(clip);
    }
    IEnumerator PlayInOrder(CustomQueue<AudioClip> clips)
    {
        AudioSource.loop = false;
        
        if (_isLoop)
        {
            while (!stopPlaying)
            {
                var clip = clips.Dequeue();
                clips.Enqueue(clip);
                yield return Play(clip);
                OnLoop.Invoke();
            }
        }
        else
        {
            var clip = clips.Dequeue();
            yield return Play(clip);
        }
        
        OnEnd.Invoke();
    }

    IEnumerator PlayInRandom(CustomQueue<AudioClip> clips)
    {
        AudioSource.loop = false;
        
        if (_isLoop)
        {
            while (!stopPlaying)
            {
                yield return StartCoroutine(PlayRandom(clips));
                OnLoop.Invoke();
            }
        }
        else
        {
            yield return StartCoroutine(PlayRandom(clips));
        }
        OnEnd.Invoke();
    }

    IEnumerator PlayIntroThenLoop(CustomQueue<AudioClip> clips)
    {
        AudioSource.loop = false;
        for (int i = 0; i < clips.Count - 1; i++)
        {
            yield return StartCoroutine(Play(clips[i]));
        }
        
        // loop clip
        var loopClip = clips[^1];

        AudioSource.clip = loopClip;
        AudioSource.loop = true;
        AudioSource.Play();

        while (!stopPlaying)
        {
            yield return null;
        }

        AudioSource.loop = false;
        AudioSource.Stop();

        OnEnd.Invoke();
    }
    void Play()
    {
        AudioSource.Play();
    }
    public void Stop(bool isImmediate = false)
    {
        stopPlaying = true;
        if (isImmediate)
        {
            AudioSource.Stop();
        }
    }
    public void Release()
    {
        if (playCoroutine != null)
        {
            StopCoroutine(playCoroutine);
            playCoroutine = null;
        }
        if (AudioSource.isPlaying)
        {
            AudioSource.clip = null;
            AudioSource.Stop();
        }

        if (gameObject.activeSelf)
        {
            GameManager.Factory.Return(gameObject);
        }
    }
}
