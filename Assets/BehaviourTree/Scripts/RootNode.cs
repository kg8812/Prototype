using UnityEngine;

namespace Apis.BehaviourTreeTool
{
    public class RootNode : TreeNode
    {
        [HideInInspector] public TreeNode child;

        public override void OnStart()
        {
            // 예전에는 여기서 트리 전체의 state를 Null로 지웠다. 에디터에 지난 결과가 남는 걸
            // 막으려던 것인데, CoolDownCheck/IfXDistance가 "자식이 아직 Running인가"를 child.state로
            // 판단하기 때문에 이 초기화가 실행 중이던 브랜치를 끊어버렸다.
            // 에디터 표시는 TreeNode.lastEvaluatedTime으로 처리하므로 여기서 건드리지 않는다.
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