using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class JururuBossMapTimeline : MonoBehaviour
{
    PlayableDirector director;
    TimelineAsset timeLine;
    private void Awake()
    {
        director = GetComponent<PlayableDirector>();
        timeLine = director.playableAsset as TimelineAsset;

        var track = timeLine.GetOutputTrack(0);
        float start = (float)track.start;
        float duration = (float)track.duration;

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => director.initialTime = start);
        seq.AppendCallback(() => director.Play());
        seq.AppendInterval(duration);
        seq.AppendCallback(() => director.Stop());
    }
}
