using UnityEngine;
using UnityEngine.Animations;

public class SetParameter : StateMachineBehaviour
{
    public enum Type
    {
        OnEnter,OnExit , OnStateMachineEnter,OnStateMachineExit
    }

    public Type stateType;
    public enum Types
    {
        Int,Float,Bool,Trigger
    }
    public Types types;
    public string paramName;
    public string value;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateType == Type.OnEnter)
        {
            switch (types)
            {
                case Types.Int:
                    animator.SetInteger(paramName, int.Parse(value));
                    break;
                case Types.Float:
                    animator.SetFloat(paramName, float.Parse(value));
                    break;
                case Types.Bool:
                    animator.SetBool(paramName, bool.Parse(value));
                    break;
                case Types.Trigger:
                    animator.SetTrigger(paramName);
                    break;
            }
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateType == Type.OnExit)
        {
            switch (types)
            {
                case Types.Int:
                    animator.SetInteger(paramName, int.Parse(value));
                    break;
                case Types.Float:
                    animator.SetFloat(paramName, float.Parse(value));
                    break;
                case Types.Bool:
                    animator.SetBool(paramName, bool.Parse(value));
                    break;
                case Types.Trigger:
                    animator.SetTrigger(paramName);
                    break;
            }
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        if (stateType == Type.OnStateMachineEnter)
        {
            switch (types)
            {
                case Types.Int:
                    animator.SetInteger(paramName, int.Parse(value));
                    break;
                case Types.Float:
                    animator.SetFloat(paramName, float.Parse(value));
                    break;
                case Types.Bool:
                    animator.SetBool(paramName, bool.Parse(value));
                    break;
                case Types.Trigger:
                    animator.SetTrigger(paramName);
                    break;
            }
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        if (stateType == Type.OnStateMachineExit)
        {
            switch (types)
            {
                case Types.Int:
                    animator.SetInteger(paramName, int.Parse(value));
                    break;
                case Types.Float:
                    animator.SetFloat(paramName, float.Parse(value));
                    break;
                case Types.Bool:
                    animator.SetBool(paramName, bool.Parse(value));
                    break;
                case Types.Trigger:
                    animator.SetTrigger(paramName);
                    break;
            }
        }
    }
}
