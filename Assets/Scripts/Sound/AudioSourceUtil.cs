using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceUtil : MonoBehaviour
{
    public enum PlayingType
    {
        Random,
        InOrder,
        IntroThenLoop
    }

    private AudioSource _audioSource;
    private UnityEvent _onEnd;
    private UnityEvent _onLoop;

    private UnityEvent _onPlay;

    private bool isLoop;

    private Coroutine playCoroutine;

    private bool stopPlaying;
    public AudioSource AudioSource => _audioSource ??= GetComponent<AudioSource>();
    public UnityEvent OnPlay => _onPlay ??= new UnityEvent();
    public UnityEvent OnLoop => _onLoop ??= new UnityEvent();
    public UnityEvent OnEnd => _onEnd ??= new UnityEvent();

    public void Play(PlayingType playingType, CustomQueue<AudioClip> clips, bool isLoop)
    {
        if (playCoroutine != null) StopCoroutine(playCoroutine);

        this.isLoop = isLoop;
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

    private IEnumerator Play(AudioClip clip)
    {
        AudioSource.Stop();
        AudioSource.clip = clip;
        AudioSource.Play();
        yield return new WaitForSecondsRealtime(clip.length);
    }

    private IEnumerator PlayRandom(CustomQueue<AudioClip> clips)
    {
        if (clips.Count == 0)
        {
            Debug.LogError("클립 개수 오류");
            yield break;
        }

        var rand = Random.Range(0, clips.Count);
        var clip = clips[rand];
        yield return Play(clip);
    }

    private IEnumerator PlayInOrder(CustomQueue<AudioClip> clips)
    {
        AudioSource.loop = false;

        if (isLoop)
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

    private IEnumerator PlayInRandom(CustomQueue<AudioClip> clips)
    {
        AudioSource.loop = false;

        if (isLoop)
            while (!stopPlaying)
            {
                yield return StartCoroutine(PlayRandom(clips));
                OnLoop.Invoke();
            }
        else
            yield return StartCoroutine(PlayRandom(clips));

        OnEnd.Invoke();
    }

    private IEnumerator PlayIntroThenLoop(CustomQueue<AudioClip> clips)
    {
        AudioSource.loop = false;
        for (var i = 0; i < clips.Count - 1; i++) yield return StartCoroutine(Play(clips[i]));

        AudioSource.loop = true;
        AudioSource.clip = clips[^1];
        Play();
    }

    public void Play()
    {
        AudioSource.Play();
        OnPlay.Invoke();
    }

    public void Stop(bool isImmediate = false)
    {
        stopPlaying = true;
        if (isImmediate) AudioSource.Stop();
    }

    public void Destroy()
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

        if (gameObject.activeSelf) GameManager.Factory.Return(gameObject);
    }
}