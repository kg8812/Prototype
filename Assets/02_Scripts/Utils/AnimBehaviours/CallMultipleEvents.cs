using System.Collections.Generic;
using UnityEngine;

public class CallMultipleEvents : StateMachineBehaviour
{
    public enum States
    {
        OnStateEnter,OnStateExit
    }

    public List<Event> events = new();
    
    [System.Serializable]
    public struct Event
    {
        public string eventName;
        public States states;

        public string value;
        public bool isParent;
        public ValueTypes valueType;
    }
   
    public enum ValueTypes
    {
        None,Int,Float,Bool,String
    }
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var x in events)
        {
            if (x.states == States.OnStateEnter)
            {
                GameObject go = x.isParent ? animator.transform.parent.gameObject : animator.gameObject;
                switch (x.valueType)
                {
                    case ValueTypes.Int:
                        go.SendMessage(x.eventName, int.Parse(x.value));
                        break;
                    case ValueTypes.Float:
                        go.SendMessage(x.eventName, float.Parse(x.value));
                        break;
                    case ValueTypes.Bool:
                        go.SendMessage(x.eventName, bool.Parse(x.value));
                        break;
                    case ValueTypes.String:
                        go.SendMessage(x.eventName, x.value);
                        break;
                    default:
                        go.SendMessage(x.eventName);
                        break;
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        foreach (var x in events)
        {
            if (x.states == States.OnStateExit)
            {
                GameObject go = x.isParent ? animator.transform.parent.gameObject : animator.gameObject;
                switch (x.valueType)
                {
                    case ValueTypes.Int:
                        go.SendMessage(x.eventName, int.Parse(x.value));
                        break;
                    case ValueTypes.Float:
                        go.SendMessage(x.eventName, float.Parse(x.value));
                        break;
                    case ValueTypes.Bool:
                        go.SendMessage(x.eventName, bool.Parse(x.value));
                        break;
                    case ValueTypes.String:
                        go.SendMessage(x.eventName, x.value);
                        break;
                    default:
                        go.SendMessage(x.eventName);
                        break;
                }
            }
        }
    }
}
