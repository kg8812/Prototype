using System.Linq;

namespace Apis.BehaviourTreeTool
{
    public class CheckOtherNode : CommonDecoratorNode
    {
        public CompositeNode CheckNode;

        public override void Init()
        {
            base.Init();
            CheckNode = tree.nodes.FirstOrDefault(node => node.guid == CheckNode.guid) as CompositeNode;
        }

        public override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            if (CheckNode == null) return State.Failure;
            var isDeco = false;
            foreach (var node in CheckNode.children)
            {
                if (node is not DecoratorNode deco) continue;
                isDeco = true;
                if (deco.Check()) return child.Update();
            }

            return isDeco ? State.Failure : child.Update();
        }

        public override bool Check()
        {
            if (CheckNode == null) return false;
            var isDeco = false;

            foreach (var node in CheckNode.children)
            {
                if (node is not DecoratorNode deco) continue;
                isDeco = true;
                if (deco.Check()) return true;
            }

            if (isDeco) return false;

            return child is DecoratorNode dec && dec.Check();
        }
    }
}