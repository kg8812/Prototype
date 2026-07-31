using Spine.Unity;
using UnityEngine;

/// <summary>
/// Spine SkeletonMecanimRootMotion 래퍼.
/// Spine을 사용하지 않는 프로젝트에서는 이 컴포넌트를 제거하면 됩니다.
/// </summary>
public class SpineRootMotionHelper : MonoBehaviour
{
    private SkeletonMecanimRootMotion rootMotion;

    private void Awake()
    {
        rootMotion = GetComponentInChildren<SkeletonMecanimRootMotion>();
        if (rootMotion == null)
            rootMotion = GetComponentInParent<SkeletonMecanimRootMotion>();
    }

    public bool IsAvailable => rootMotion != null;

    public void SetOffset(float x, float y)
    {
        if (rootMotion == null) return;

        rootMotion.rootMotionTranslateXPerY = x;
        rootMotion.rootMotionTranslateYPerX = y;
    }

    public Vector2 GetOffset()
    {
        if (rootMotion == null) return Vector2.zero;

        return new Vector2(rootMotion.rootMotionTranslateXPerY, rootMotion.rootMotionTranslateYPerX);
    }

    public void AddOffset(float x, float y)
    {
        if (rootMotion == null) return;

        rootMotion.rootMotionTranslateXPerY += x;
        rootMotion.rootMotionTranslateYPerX += y;
    }

    public void SetScale(float x, float y)
    {
        if (rootMotion == null) return;

        rootMotion.rootMotionScaleX = x;
        rootMotion.rootMotionScaleY = y;
    }

    public Vector2 GetScale()
    {
        if (rootMotion == null) return Vector2.zero;

        return new Vector2(rootMotion.rootMotionScaleX, rootMotion.rootMotionScaleY);
    }
}
