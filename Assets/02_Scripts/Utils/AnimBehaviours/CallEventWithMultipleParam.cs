using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

public class CallEventWithMultipleParam : StateMachineBehaviour
{
    public string eventName;
    public enum States
    {
        OnStateEnter,OnStateExit,OnStateMachineEnter,OnStateMachineExit
    }

    public States states;

    public bool isParent;
    public enum ValueTypes
    {
        None,Int,Float,Bool,String
    }
    public ValueTypes valueType;

    public string value;
    public int valueCount;

    void SendEvent(Animator animator)
    {
        GameObject go = isParent ? animator.transform.parent.gameObject : animator.gameObject;
        string[] strs = value.Split(',');
        switch (valueType)
        {
            case ValueTypes.Int:
                int[] ints = new int[valueCount];
                for (int i = 0; i < valueCount; i++)
                {
                    ints[i] = int.Parse(strs[i]);
                }
                go.SendMessage(eventName, ints);
                break;
            case ValueTypes.Float:
                float[] floats = new float[valueCount];
                for (int i = 0; i < valueCount; i++)
                {
                    floats[i] = float.Parse(strs[i]);
                }
                go.SendMessage(eventName, floats);
                break;
            case ValueTypes.Bool:
                bool[] bools = new bool[valueCount];
                for (int i = 0; i < valueCount; i++)
                {
                    bools[i] = bool.Parse(strs[i]);
                }
                go.SendMessage(eventName, bools);
                break;
            case ValueTypes.String:
                go.SendMessage(eventName, strs);
                break;
            default:
                go.SendMessage(eventName);
                break;
        }
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == States.OnStateEnter)
        {
           SendEvent(animator); 
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == States.OnStateExit)
        {
            SendEvent(animator); 
        }
    }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        if (states == States.OnStateMachineEnter)
        {
            SendEvent(animator); 
        }
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (states == States.OnStateMachineExit)
        {
            SendEvent(animator); 
        }
    }
}
