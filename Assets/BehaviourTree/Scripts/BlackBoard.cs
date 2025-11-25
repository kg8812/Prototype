using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    [System.Serializable]
    public class BlackBoard
    {
        public Tweener tweener;
        [HideInInspector] public string currentNodeName;
        [HideInInspector] public TreeNode currentNode;
    }
}