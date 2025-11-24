using Default;
using UnityEngine;

public class AnimClimbBehaviour : BossRootMotionBehaviour
{
    private Player player;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        player = animator.transform.GetComponentInParentAndChild<Player>();
        player.StateEvent.ExecuteEventOnce(EventType.OnClimbMotionStart, null);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if(!animator.IsInTransition(0)){
            // player.StateEvent.ExecuteEventOnce(EventType.OnClimbMotionStart, null);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
