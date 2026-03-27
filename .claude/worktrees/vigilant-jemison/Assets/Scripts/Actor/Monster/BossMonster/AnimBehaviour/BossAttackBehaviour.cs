using Apis;
using Default;
using UnityEngine;

public class BossAttackBehaviour : StateMachineBehaviour
{
    public int patternNumber;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        var boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
        if (boss != null) boss.currentAtkPattern = patternNumber;
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);
        animator.SetInteger("Attack", 0);
        animator.SetInteger("AttackType", 0);
        var boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
        if (boss != null)
        {
            boss.treeRunner.Alert("AttackEnd");
            boss.EndAttack(patternNumber);
            boss.currentAtkPattern = 0;
        }
    }
}