using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackEvent(Lerp)", menuName = "AttackEvent/Lerp")]
public class LerpEvent : AttackEvent
{
    public float time;
    public float velocity;
    public override void Invoke(Player p)
    {
        p.Stop();

        Vector2 dir = p.GetSlope().normalized;

        var tweener = p.Rb.DOMove(time * velocity * dir, velocity)
                        .SetUpdate(UpdateType.Fixed)
                        .SetSpeedBased()
                        .SetRelative();
        tweener.onComplete += () => tweener.Kill();

        p.StateEvent.AddEvent(EventType.OnAttackStateExit, (e) => tweener.Kill());

        if(BlendLegAction && p.PressingDir != 0) p.AnimController.SetTrigger(EAnimationTrigger.StepAttack);
    }
}
