using System;
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

        /// <summary>
        ///     이 노드가 마지막으로 평가된 시각. state는 "마지막 결과"라서 평가되지 않은 노드에도 계속 남는다.
        ///     지금 틱에 실제로 지나간 노드만 에디터가 색칠하도록 구분하는 용도이며, 런타임 판정에는 쓰지 않는다.
        /// </summary>
        [NonSerialized] public float lastEvaluatedTime = float.NegativeInfinity;
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
            lastEvaluatedTime = Time.time;

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

        /// <summary>
        ///     실행 중이던 노드를 중단하고, 다음 진입 때 OnStart부터 다시 시작하도록 자신과 하위 노드를 되돌린다.
        ///     state만 덮어쓰면 isStarted가 true로 남아 OnStart가 건너뛰어지므로 반드시 이 경로로 중단할 것.
        /// </summary>
        public void Abort()
        {
            if (isStarted)
            {
                OnStop();
                isStarted = false;
            }

            // 중단은 "실패"가 아니라 "평가되지 않음"이다. Failure로 덮으면 실행된 적도 없는
            // 하위 노드까지 에디터에서 전부 빨갛게 뜬다.
            state = State.Null;

            BehaviourTree.GetChildren(this).ForEach(child => child.Abort());
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