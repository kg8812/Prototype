using UnityEngine;

public class CallEventWhenAnimationEnd : StateMachineBehaviour
{
    public string eventName;
    private bool eventCalled;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        eventCalled = false; // 다음 진입을 위해 초기화
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // normalizedTime이 1 이상이면 애니메이션이 끝난 것
        if (!eventCalled && stateInfo.normalizedTime >= 1.0f)
        {
            eventCalled = true;

            // 원하는 이벤트 호출
            animator.gameObject.SendMessage(eventName);
        }
    }
}