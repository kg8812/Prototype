public interface IState<T>
{
    // 상태 인터페이스

    // 상태를 구현할 때 클래스로 새로 구현하여서 해당 클래스를 상속 받아서 사용.
    // 제네릭에 해당 상태를 사용할 클래스 기입 (플레이어, 몬스터 등)

    void OnEnter(T t); // 상태에 들어갈 때 호출하는 함수
    void Update(); // 상태의 업데이트 함수
    void FixedUpdate(); // 상태의 물리용 업데이트 함수
    void OnExit(); // 상태에서 벗어날 때 호출하는 함수
}