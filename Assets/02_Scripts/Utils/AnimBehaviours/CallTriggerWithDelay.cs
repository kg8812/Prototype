using System.Collections;
using UnityEngine;

public class CallTriggerWithDelay: StateMachineBehaviour
{
    public string triggerName = "loopEnd";
    public float delayTime = 2f;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameManager.instance.StartCoroutineWrapper(SetTriggerDelay(animator));
    }

    private IEnumerator SetTriggerDelay(Animator anim)
    {
        yield return new WaitForSeconds(delayTime);
        anim.SetTrigger(triggerName);
    }
}