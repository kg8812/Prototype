using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    [System.Serializable]
    public class BlackBoard
    {
        public JururuBoss jururuBoss;
        public Tweener tweener;
        [HideInInspector] public string currentNodeName;
        [HideInInspector] public TreeNode currentNode;

        [System.Serializable]
        public class JururuBoss
        {
            public enum Patterns
            {
                Walk, Dash, BackDash, Attack1, Attack2, Attack3, Attack4, Attack5,
                None
            }

            public Patterns pattern;
            public readonly List<Actor> summons = new();
        }
    }
}