using System;
using DG.Tweening;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        public enum StartType
        {
            OnStart,
            ByScript
        }
        public BehaviourTree tree;
        [HideInInspector] public Actor actor;
        public bool Repeat;

        public StartType startType;

        bool isStarted;

        public UpdateType updateType = UpdateType.Normal;

        public void StartRunningTree()
        {
            isStarted = true;
        }

        public void Stop()
        {
            isStarted = false;
        }

        private void Awake()
        {
            switch (startType)
            {
                case StartType.OnStart:
                    isStarted = true;
                    break;
                case StartType.ByScript:
                    isStarted = false;
                    break;
            }
            actor = GetComponent<Actor>();
            tree = tree.Clone();
            tree.Init(actor, Repeat);
        }
        
        private void Update()
        {
            if (isStarted && updateType == UpdateType.Normal)
            {
                tree.Update();
            }
        }

        private void FixedUpdate()
        {
            if(isStarted && updateType == UpdateType.Fixed) 
            {
                tree.Update();
            }
        }

        private void LateUpdate()
        {
            if (isStarted && updateType == UpdateType.Late)
            {
                tree.Update();
            }
        }
        public void Alert(string message)
        {
            BehaviourTree.Traverse(tree.rootNode, (x) =>
            {
                x.OnAlert.Invoke(message);
            });
        }
    }
}