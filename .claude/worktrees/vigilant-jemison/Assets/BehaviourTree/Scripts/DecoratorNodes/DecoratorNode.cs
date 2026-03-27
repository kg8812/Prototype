using Sirenix.OdinInspector;

namespace Apis.BehaviourTreeTool
{
    public abstract class DecoratorNode : TreeNode
    {
        [ReadOnly] public TreeNode child;

        protected bool CheckChild
        {
            get
            {
                if (child != null && child is DecoratorNode dec) return dec.Check();
                return child != null;
            }
        }

        public override TreeNode Clone()
        {
            var node = Instantiate(this);
            if (child != null) node.child = child.Clone();
            return node;
        }

        public abstract bool Check();
    }
}