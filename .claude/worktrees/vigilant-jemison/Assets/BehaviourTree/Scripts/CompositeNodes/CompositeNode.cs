using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Apis.BehaviourTreeTool
{
    public abstract class CompositeNode : TreeNode
    {
        [ReadOnly] public List<TreeNode> children = new();

        public override TreeNode Clone()
        {
            var node = Instantiate(this);
            node.children = children.ConvertAll(x => x.Clone());
            return node;
        }

        public override void OnStart()
        {
        }
    }
}