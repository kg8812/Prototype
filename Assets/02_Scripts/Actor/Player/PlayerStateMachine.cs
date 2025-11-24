using System.Collections;
using System.Collections.Generic;
using Apis;
using PlayerState;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerStateMachine : StateMachine<Player>
{
    private Player _player;
    private Coroutine interruptCoroutine;
    public PlayerStateMachine(Player t, IState<Player> currentState) : base(t, currentState)
    {
        _player = t;
    }

    public void SubRoutine()
    {
        if(CurrentState is IAutoEscape)
        {
            var state = CurrentState as IAutoEscape;
            if(state.EscapeCondition()) _player.SetState(state.NextState);
        }

    }

    public override void SetState(IState<Player> state)
    {
        if (CurrentState == state) return;
        // 이전 interrupt state 동작 정지
        if(interruptCoroutine != null) GameManager.instance.StopCoroutineWrapper(interruptCoroutine);

        CurrentState.OnExit();
        
        IAnimate animate = CurrentState as IAnimate;
        animate?.OnExitAnimate();

        CurrentState = state;
        CurrentState.OnEnter(_player);

        animate = CurrentState as IAnimate;
        animate?.OnEnterAnimate();

        if(CurrentState is IInterruptable istate && istate.InterruptTime >= 0)
            interruptCoroutine = GameManager.instance.StartCoroutineWrapper(Interrupt(CurrentState as BaseState));

    }

    public void ResetInterrupt(float time)
    {
        if(CurrentState is not IInterruptable) return;

        if(interruptCoroutine != null) GameManager.instance.StopCoroutineWrapper(interruptCoroutine);

        interruptCoroutine = GameManager.instance.StartCoroutineWrapper(Interrupt(CurrentState as BaseState, time));
    }

    private IEnumerator Interrupt(BaseState state, float time = -1)
    {
        var interruptable = CurrentState as IInterruptable;

        float t = time < 0 ? interruptable.InterruptTime : time;

        yield return new WaitForSeconds(t);
        foreach(var next in interruptable.InteruptableStates)
        {
            _player.SetAbleState(next);
        }
        state.AbleStates.AddRange(interruptable.InteruptableStates);
    }
}
