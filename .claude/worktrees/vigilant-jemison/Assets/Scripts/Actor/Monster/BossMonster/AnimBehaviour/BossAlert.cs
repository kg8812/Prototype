using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

public class BossAlert : StateMachineBehaviour
{
    public enum State
    {
        OnEnter,
        OnExit,
        OnStateMachineEnter,
        OnStateMachineExit
    }

    [LabelText("실행 시기")] public State state;

    public string str;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == State.OnEnter)
        {
            var boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null) boss.treeRunner.Alert(str);
        }
    }

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == State.OnExit)
        {
            var boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null) boss.treeRunner.Alert(str);
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (state == State.OnStateMachineEnter)
        {
            var boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null) boss.treeRunner.Alert(str);
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (state == State.OnStateMachineExit)
        {
            var boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null) boss.treeRunner.Alert(str);
        }
    }
}