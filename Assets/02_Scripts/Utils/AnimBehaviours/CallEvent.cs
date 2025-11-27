using UnityEngine;
using UnityEngine.Animations;

public class CallEvent : StateMachineBehaviour
{
    public enum States
    {
        OnStateEnter,
        OnStateExit,
        OnStateMachineEnter,
        OnStateMachineExit
    }

    public enum ValueTypes
    {
        None,
        Int,
        Float,
        Bool,
        String
    }

    public string eventName;

    public States states;

    public string value;
    public bool isParent;
    public ValueTypes valueType;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == States.OnStateEnter)
        {
            var go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
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
            var go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
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

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash,
        AnimatorControllerPlayable controller)
    {
        if (states == States.OnStateMachineEnter)
        {
            var go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
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
            var go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
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