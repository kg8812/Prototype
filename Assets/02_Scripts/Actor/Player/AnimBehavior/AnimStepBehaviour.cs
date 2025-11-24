using UnityEngine;

public class AnimStepBehaviour : StateMachineBehaviour
{
    Player _player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _player = animator.transform.parent.GetComponent<Player>();
    }

}
