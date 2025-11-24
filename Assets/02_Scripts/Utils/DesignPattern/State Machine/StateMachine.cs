using UnityEngine;

public class StateMachine<T>
{
    // 상태기계 클래스
    // IState와 동일하게 제네릭에 상태를 사용할 클래스 기입

    public IState<T> CurrentState { get; protected set; }
    readonly T t;

    public StateMachine(T t, IState<T> currentState) // 생성자. 사용할 객체와 시작 상태 기입
    {
        CurrentState = currentState;
        CurrentState.OnEnter(t);
        this.t = t;
    }

    public virtual void SetState(IState<T> state) // 상태 변경 함수
    {
        // 이미 해당 상태일시 함수 종료
        if (CurrentState == state)
        {
            // Debug.Log("이미 해당 상태임");
            return;
        }

        // 변경 전 상태의 Exit, 변경 후 상태의 Enter 호출
        CurrentState.OnExit();

        CurrentState = state;

        CurrentState.OnEnter(t);
    }

    // 업데이트 함수, 사용하는 객체의 update 함수에서 호출. Monobehaviour 상속 안받아서 호출 해줘야함
    public void Update()
    {
        CurrentState.Update();
    }

    //Fixed Update 함수, 사용하는 객체의 Fixed Update 함수에서 호출.
    public void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }
}
