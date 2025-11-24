using Spine.Unity;
using UnityEngine;
using DG.Tweening;
using Spine;
using Unity.Mathematics;
using Sequence = DG.Tweening.Sequence;

public class GoseguSpineTest : MonoBehaviour
{
    [SpineAnimation] public string[] attack; // 애니메이션 목록
    private SkeletonAnimation _animation; // Skeleton Animation 컴포넌트
    private Spine.AnimationState animationState; // 애니메이션 제어를 위한 SkeletonAnimation 내부 State 클래스

    private void Start()
    {
        _animation = GetComponent<SkeletonAnimation>();
        animationState = _animation.AnimationState;
        Attack();
    }

    void Attack() // 공격 애니메이션 실행
    {
        animationState.SetAnimation(0, attack[0], false);
        animationState.AddAnimation(0, attack[1], false, 0).Start += Jump;
        animationState.AddAnimation(0, attack[2], false, moveTime2);
        animationState.AddAnimation(0, attack[3], false, 0);
    }

    private void Update()
    {
    }

    private Sequence seq;

    public float moveTime2;
    public float jumpHeight2;
    public float distance2;
    public Ease ease2;

    private void Jump(TrackEntry entry)
    {
        seq?.Kill();

        seq = DoJump(moveTime2, jumpHeight2, -distance2)
            .SetEase(ease2);
        seq.onComplete += () => transform.rotation = quaternion.identity;
    }

    public Sequence DoJump(float time, float height, float distance)
    {
        float temp = distance;

        Vector2 position = transform.position;

        float x = position.x + temp;
        Vector2 pos = new Vector2(x, position.y) + Vector2.right * temp;

        return transform.DOJump(pos, height, 1, time).SetUpdate(UpdateType.Fixed);
    }
}