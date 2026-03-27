using System;
using DG.Tweening;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    [Serializable]
    public class BlackBoard
    {
        [HideInInspector] public string currentNodeName;
        [HideInInspector] public TreeNode currentNode;
        public Tweener tweener;
    }
}