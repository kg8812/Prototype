using UnityEngine;
using UnityEngine.Animations;

public class AnimAttackBehaviour : StateMachineBehaviour
{
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    Player _player;
    // override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    // {
    //
    //   if(_player == null) return;
    //
    // //   _player.DashOff();
    // //   _player.AttackOff();
    // //   _player.MoveComponent.JumpOff();
    // //   animator.SetInteger("IncomingState", (int)EPlayerState.Attack);
    // }

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash, AnimatorControllerPlayable controller)
    {
        base.OnStateMachineEnter(animator, stateMachinePathHash, controller);
        _player = animator.transform.parent.GetComponent<Player>();
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        base.OnStateMachineExit(animator, stateMachinePathHash);
        _player?.AfterWeaponAtk();
        if (_player != null)
        {
            _player.weaponAtkInfo.atkCombo = 0;
        }
    }
}
