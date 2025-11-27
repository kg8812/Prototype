using DG.Tweening;
using UnityEngine;

namespace Apis
{
    public class RepeatMovingObj : MonoBehaviour
    {
        [SerializeField] private bool isUpDown;

        [SerializeField] private float dist;
        [SerializeField] private float duration;
        [SerializeField] private Ease type;


        private void Start()
        {
            // 시작 위치를 기준으로 좌우로 이동
            var toPos = transform.position + (isUpDown ? Vector3.up : Vector3.right) * dist;
            // Vector3 originPos = transform.position;

            // DoTween을 사용하여 좌우로 반복 이동
            transform.DOMove(toPos, duration)
                .SetEase(type)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }
}