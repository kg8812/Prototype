using Default;
using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;

public class BossRootMotionBehaviour : StateMachineBehaviour
{
    [LabelText("x 거리비율")][Tooltip("0이면 기능이 꺼집니다")] public float xScale;
    [LabelText("y 거리비율")][Tooltip("0이면 기능이 꺼집니다")] public float yScale;
    [LabelText("x 거리 offset")][Tooltip("입력한 수치만큼 모션에 맞춰 x축으로 더 이동합니다")] public float xDistance;
    [LabelText("y 거리 offset")][Tooltip("입력한 수치만큼 모션에 맞춰 y축으로 더 이동합니다")] public float yDistance;
    [LabelText("rotation 사용여부")] public bool useRotation;

    [LabelText("모션 종료시 root motion 비활성화 여부")]
    public bool disableRootMotion;

    private SkeletonMecanimRootMotion rootMotion;

    void SetRootMotion(Animator animator)
    {
        if (rootMotion != null) return;

        rootMotion = animator.transform.GetComponentInParentAndChild<SkeletonMecanimRootMotion>();
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        SetRootMotion(animator);
        animator.applyRootMotion = true;
        rootMotion.enabled = true;
        rootMotion.rootMotionScaleX = xScale;
        rootMotion.rootMotionScaleY = yScale;
        rootMotion.transformPositionX = !Mathf.Approximately(xScale,0);
        rootMotion.transformPositionY = !Mathf.Approximately(yScale,0);
        rootMotion.transformRotation = useRotation;
        rootMotion.rootMotionTranslateXPerY = xDistance;
        rootMotion.rootMotionTranslateYPerX = yDistance;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (disableRootMotion)
        {
            SetRootMotion(animator);
            animator.applyRootMotion = false;
            rootMotion.enabled = false;
            rootMotion.transformPositionX = false;
            rootMotion.transformPositionY = false;
            rootMotion.transformRotation = false;
            rootMotion.rootMotionScaleX = 1;
            rootMotion.rootMotionScaleY = 1;
            rootMotion.rootMotionTranslateXPerY = 0;
            rootMotion.rootMotionTranslateYPerX = 0;
        }
    }
}