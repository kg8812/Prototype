using System.Collections;
using System.Collections.Generic;
using Apis;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class WayPointPlatform : MonoBehaviour
{
    [SerializeField] private MovingObj platform;

    [FormerlySerializedAs("moveSpeed")] [LabelText("이동시간")]
    public float moveTime;

    [LabelText("대기시간")] public float delay;

    [InfoBox("움직이는 순서대로 넣되, 초기 위치를 마지막에 넣어주세요. 마지막에 초기 위치로 돌아와야하기 때문입니다.")] [LabelText("이동 위치")]
    public List<Transform> wayPoints;


    private void Awake()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
            for (var i = 0; i < wayPoints.Count; i++)
            {
                Tween tween = platform.transform.DOMove(wayPoints[i].position, moveTime)
                    .SetEase(Ease.Linear)
                    .SetUpdate(UpdateType.Fixed);

                yield return tween.WaitForCompletion();
                yield return new WaitForSeconds(delay);
            }
        // ReSharper disable once IteratorNeverReturns
    }
}