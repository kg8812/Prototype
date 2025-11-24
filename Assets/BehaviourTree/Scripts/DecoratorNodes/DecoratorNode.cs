using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public abstract class DecoratorNode : TreeNode
    {
        [ReadOnly] public TreeNode child;

        public override TreeNode Clone()
        {
            DecoratorNode node = Instantiate(this);
            if (child != null)
            {
                node.child = child.Clone();
            }
            return node;
        }

        public abstract bool Check();

        protected bool CheckChild
        {
            get
            {
                if(child != null && child is DecoratorNode dec)
                {
                    return dec.Check();
                }
                return child != null;
            }
        }
    }
}