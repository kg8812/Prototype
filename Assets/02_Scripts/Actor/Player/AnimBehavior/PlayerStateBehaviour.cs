using Default;
using UnityEngine;

public class PlayerStateBehaviour : StateMachineBehaviour
{
    public EPlayerState state;
    
    Player _player;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player ??= animator.transform.GetComponentInParentAndChild<Player>();
        _player?.SetState(state);
    }

}
