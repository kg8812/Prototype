using Default;
using UnityEngine;

public class MoveBehaviour : StateMachineBehaviour
{
    private Actor actor;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (actor == null) actor = Utils.GetComponentInParentAndChild<Actor>(animator.gameObject);

        if (actor == null) return;

        animator.SetFloat("MoveMultiplier",
            1 * actor.MoveSpeed / actor.StatManager.BaseStat.Get(ActorStatType.MoveSpeed));
    }
}
