using UnityEngine;
using UnityEngine.Animations;

public class CallEvent : StateMachineBehaviour
{
    public string eventName;
    public enum States
    {
        OnStateEnter,OnStateExit,OnStateMachineEnter,OnStateMachineExit
    }

    public States states;

    public string value;
    public bool isParent;
    public enum ValueTypes
    {
        None,Int,Float,Bool,String
    }
    public ValueTypes valueType;
    
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == States.OnStateEnter)
        {
            GameObject go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
            switch (valueType)
            {
                case ValueTypes.Int:
                    go.SendMessage(eventName, int.Parse(value));
                    break;
                case ValueTypes.Float:
                    go.SendMessage(eventName, float.Parse(value));
                    break;
                case ValueTypes.Bool:
                    go.SendMessage(eventName, bool.Parse(value));
                    break;
                case ValueTypes.String:
                    go.SendMessage(eventName, value);
                    break;
                default:
                    go.SendMessage(eventName);
                    break;
            }
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == States.OnStateExit)
        {
            GameObject go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
            switch (valueType)
            {
                case ValueTypes.Int:
                    go.SendMessage(eventName, int.Parse(value));
                    break;
                case ValueTypes.Float:
                    go.SendMessage(eventName, float.Parse(value));
                    break;
                case ValueTypes.Bool:
                    go.SendMessage(eventName, bool.Parse(value));
                    break;
                case ValueTypes.String:
                    go.SendMessage(eventName, value);
                    break;
                default:
                    go.SendMessage(eventName);
                    break;
            }
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        if (states == States.OnStateMachineEnter)
        {
            GameObject go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
            switch (valueType)
            {
                case ValueTypes.Int:
                    go.SendMessage(eventName, int.Parse(value));
                    break;
                case ValueTypes.Float:
                    go.SendMessage(eventName, float.Parse(value));
                    break;
                case ValueTypes.Bool:
                    go.SendMessage(eventName, bool.Parse(value));
                    break;
                case ValueTypes.String:
                    go.SendMessage(eventName, value);
                    break;
                default:
                    go.SendMessage(eventName);
                    break;
            }
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (states == States.OnStateMachineExit)
        {
            GameObject go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
            switch (valueType)
            {
                case ValueTypes.Int:
                    go.SendMessage(eventName, int.Parse(value));
                    break;
                case ValueTypes.Float:
                    go.SendMessage(eventName, float.Parse(value));
                    break;
                case ValueTypes.Bool:
                    go.SendMessage(eventName, bool.Parse(value));
                    break;
                case ValueTypes.String:
                    go.SendMessage(eventName, value);
                    break;
                default:
                    go.SendMessage(eventName);
                    break;
            }
        }
    }
}
