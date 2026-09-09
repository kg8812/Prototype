namespace Apis.BehaviourTreeTool
{
    /// <summary>
    ///     우선순위 선택 노드. 매 틱 0번 자식부터 다시 평가하므로, 하위 우선순위가 Running인 도중에도
    ///     상위 우선순위 조건이 성립하면 즉시 그쪽으로 전환된다(reactive).
    ///     전환 시 실행 중이던 하위 브랜치는 Abort로 정리해 다음 진입 때 OnStart부터 다시 시작한다.
    /// </summary>
    public class SelectNode : CommonCompositeNode
    {
        private int runningIndex = -1;

        public override void OnStart()
        {
            base.OnStart();
            runningIndex = -1;
        }

        public override void OnStop()
        {
        }

        public override State OnUpdate()
        {
            for (var i = 0; i < children.Count; i++)
            {
                var state = children[i].Update();
                if (state == State.Failure)
                {
                    // 실행 중이던 자식이 스스로 실패했다면 이미 정리된 상태다.
                    // 그대로 두면 아래에서 끝난 브랜치를 다시 Abort해 OnStop이 두 번 돈다.
                    if (runningIndex == i) runningIndex = -1;
                    continue;
                }

                // 실행 브랜치가 바뀌었다면 이전 브랜치의 잔여 상태를 정리한다
                if (runningIndex >= 0 && runningIndex != i) children[runningIndex].Abort();

                runningIndex = state == State.Running ? i : -1;
                return state;
            }

            if (runningIndex >= 0)
            {
                children[runningIndex].Abort();
                runningIndex = -1;
            }

            return State.Failure;
        }
    }
}
