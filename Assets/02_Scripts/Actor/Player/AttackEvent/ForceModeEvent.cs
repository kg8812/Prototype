using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackEvent(Force)", menuName = "AttackEvent/ForceMode")]
public class ForceModeEvent : AttackEvent
{
    public float angle;
    public float magnitude;
    public bool isReverse = false;
    public bool isFlip = false;

    public override void Invoke(Player p)
    {
        p.Stop();
        float a = Mathf.Deg2Rad * angle;
        Vector2 dir = new(Mathf.Cos(a) * (int)p.Direction, Mathf.Sin(a));

        if(isReverse) 
            dir *= -1;

        if(isFlip)
            p.Flip();

        p.Rb.AddForce(dir * magnitude, ForceMode2D.Impulse);

        if(BlendLegAction && p.PressingDir != 0) p.AnimController.SetTrigger(EAnimationTrigger.StepAttack);
    }
}
