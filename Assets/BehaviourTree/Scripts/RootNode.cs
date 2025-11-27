using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class RootNode : TreeNode
    {
        [HideInInspector] public TreeNode child;

        public override void OnStart()
        {
            BehaviourTree.Traverse(this, n => { n.state = State.Null; });
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (child == null) return State.Running;

            return child.Update();
        }

        public override TreeNode Clone()
        {
            var node = Instantiate(this);

            if (child != null) node.child = child.Clone();

            return node;
        }
    }
}