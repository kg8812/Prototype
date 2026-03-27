using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Apis.BehaviourTreeTool
{
    public abstract class TreeNode : ScriptableObject
    {
        public enum State // 노드 상태
        {
            Running,
            Failure,
            Success,
            Null
        }

        [HideInInspector] public State state;
        [HideInInspector] public bool isStarted;
        [HideInInspector] public string guid;
        [HideInInspector] public Vector2 position;
        [TextArea] public string description;

        [HideInInspector] public BlackBoard blackBoard;

        [FormerlySerializedAs("actor")] [HideInInspector]
        public Actor _actor;

        [HideInInspector] public BehaviourTree tree;
        [HideInInspector] public UnityEvent<string> OnAlert = new();

        public virtual State Update()
        {
            if (!isStarted)
            {
                OnStart();
                isStarted = true;
            }

            state = OnUpdate();
            if (state == State.Failure || state == State.Success)
            {
                OnStop();
                isStarted = false;
            }

            return state;
        }

        public virtual void Init()
        {
        }

        public virtual TreeNode Clone()
        {
            return Instantiate(this);
        }

        public abstract void OnStart();
        public abstract void OnStop();
        public abstract State OnUpdate();

        public virtual void OnSkip()
        {
        }

        public virtual void SetActor(Actor actor)
        {
            _actor = actor;
        }

        public IEnumerator InvokeInTime(float time, UnityAction action)
        {
            yield return new WaitForSeconds(time);
            action.Invoke();
        }
    }
}