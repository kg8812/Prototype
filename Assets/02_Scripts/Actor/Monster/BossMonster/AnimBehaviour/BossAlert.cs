using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

public class BossAlert : StateMachineBehaviour
{
    public enum State
    {
        OnEnter,
        OnExit,
        OnStateMachineEnter,
        OnStateMachineExit,
    }

    [LabelText("실행 시기")] public State state;

    public string str;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == State.OnEnter)
        {
            BossMonster boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null)
            {
                boss.treeRunner.Alert(str);
            }
        }
    }
    
    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (state == State.OnExit)
        {
            BossMonster boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null)
            {
                boss.treeRunner.Alert(str);
            }
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (state == State.OnStateMachineEnter)
        {
            BossMonster boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null)
            {
                boss.treeRunner.Alert(str);
            }
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {

        if (state == State.OnStateMachineExit)
        {
            BossMonster boss = Utils.GetComponentInParentAndChild<BossMonster>(animator.gameObject);
            if (boss != null)
            {
                boss.treeRunner.Alert(str);
            }
        }
    }
}