namespace Apis.BehaviourTreeTool
{
    /// <summary>
    ///     Rigidbody 속도로 액터를 계속 밀어 움직이는 노드의 기반 클래스.
    ///     ActionNode의 기본 동작은 진입 시 속도를 0으로 만드는 것인데(공격 전 정지 목적),
    ///     이동 노드가 그 규칙을 그대로 받으면 매 진입마다 가속이 초기화되어
    ///     ActorMovement의 lerp 가속이 누적되지 못한다. 그래서 여기서만 정지를 끈다.
    ///     ActionNode를 직접 상속하면 에디터 노드 메뉴에서 별도 카테고리로 빠지므로
    ///     CommonActionNode 아래에 둔다.
    /// </summary>
    public abstract class MovementActionNode : CommonActionNode
    {
        protected override bool StopOnStart => false;
    }
}
