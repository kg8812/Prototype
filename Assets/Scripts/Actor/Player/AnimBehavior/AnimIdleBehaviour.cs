using UnityEngine;

public class AnimIdleBehaviour : StateMachineBehaviour
{
    private Player _player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player ??= animator.GetComponentInParent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player component not found on the parent of the Animator.");
            return;
        }

        _player.IsReadyIdle = true;
        _player.StateEvent.ExecuteEventOnce(EventType.OnIdleMotion, null);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_player == null) return;
        _player.IsReadyIdle = false;
    }
}
