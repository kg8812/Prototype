using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using Spine.Unity;
using UnityEngine;

public class BossAttackBehaviour : StateMachineBehaviour
{
    public int patternNumber;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        BossMonster boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
        if (boss != null)
        {
            boss.currentAtkPattern = patternNumber;
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);
        animator.SetInteger("Attack",0);
        animator.SetInteger("AttackType",0);
        BossMonster boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
        if (boss != null)
        {
            boss.treeRunner.Alert("AttackEnd");
            boss.RootMotion.enabled = false;
            boss.EndAttack(patternNumber);
            boss.currentAtkPattern = 0;
        }
    }
}
