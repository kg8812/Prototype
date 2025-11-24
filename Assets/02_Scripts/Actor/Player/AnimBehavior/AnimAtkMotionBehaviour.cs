using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimAtkMotionBehaviour : StateMachineBehaviour
{
    public int atkCombo;

    public enum AirOrGround
    {
        Air,
        Ground
    }

    public AirOrGround atkType;

    public bool isWeaponAtk;

    [HideIf("isWeaponAtk")]
    public bool isFinalAttack;
    private Player p = null;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.transform.parent != null)
        {
            actor = animator.transform.parent.GetComponentInChildren<IEventUser>(false);
        }
        else
        {
            actor = animator.transform.GetComponentInChildren<IEventUser>(false);
        }

        Actor realActor = Default.Utils.GetComponentInParentAndChild<Actor>(animator.gameObject);
        if (realActor != null)
        {
            animator.SetFloat("AtkSpeed", 1 + realActor.AtkSpeed / 100);
            if (realActor is Player player)
            {
                p = player;
                int max;
                player.weaponAtkInfo.atkCombo = atkCombo;
                player.weaponAtkInfo.airOrGround = atkType;
                Weapon weapon = AttackItemManager.CurrentItem as Weapon;

                if (isWeaponAtk && weapon == null)
                {
                    Debug.LogError("무기가 할당되지 않았습니다.");
                    return;
                }
                // if (isWeaponAtk)
                // {
                //     if (WeaponData.DataLoad.TryGetMotionGroup(weapon.MotionIndex, out var data))
                //     {
                //         if (WeaponData.DataLoad.TryGetMotion(data.groundMotions[atkCombo - 1], out var d))
                //         {
                //             foreach (var sprite in weapon.wpSprites)
                //             {
                //                 if (sprite.TryGetComponent(out SpriteRenderer render))
                //                 {
                //                     render.flipY =
                //                         d.motionName is "spear_attack_1" or "spear_attack_3" or "spear_attack_4";
                //                 }
                //             }
                //         }
                //     }
                // }

                if (isWeaponAtk)
                {
                    weapon.OnAnimEnter(atkCombo);
                    player.Slash();
                }

                // player.StopAttackEscapeCoolDown();
                switch (atkType)
                {
                    case AirOrGround.Ground:
                        max = animator.GetInteger("MaxGroundAtk");
                        if (isWeaponAtk)
                        {
                            if (weapon.attackType == Weapon.AttackType.Collider)
                            {
                                weapon.SetGroundCollider(atkCombo - 1, player.attackColliders);
                            }
                        }

                        // player.StartAttackEscapeCoolDown(player.attackStrategy.GroundAttackEscapeTime(atkCombo - 1));
                        // player.StateMachine.ResetInterrupt(player.attackStrategy.GroundAttackEscapeTime(atkCombo - 1));
                        break;
                    case AirOrGround.Air:
                        max = animator.GetInteger("MaxAirAtk");

                        if (isWeaponAtk)
                        {
                            if (weapon.attackType == Weapon.AttackType.Collider)
                            {
                                weapon.SetAirCollider(atkCombo - 1, player.attackColliders);
                            }
                        }

                        // player.StartAttackEscapeCoolDown(player.attackStrategy.AirAttackEscapeTime(atkCombo - 1));
                        // player.StateMachine.ResetInterrupt(player.attackStrategy.AirAttackEscapeTime(atkCombo - 1));
                        break;
                    default:
                        max = 0;
                        break;
                }

                if (atkCombo == max || isFinalAttack)
                {
                    if (player == null)
                        return;
                    player.OnFinalAttack = true;
                }
                p.StateEvent.ExecuteEventOnce(EventType.OnAttackSuccess, null);
            }
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if(p == null) return;
        // if(!animator.IsInTransition(0))
        //     p.StateEvent.ExecuteEventOnce(EventType.OnAttackSuccess, null);
    }

    IEventUser actor;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actor.EventManager.ExecuteEvent(EventType.OnAttackEnd, new EventParameters(actor));

        if (actor is Player player && AttackItemManager.CurrentItem is Weapon weapon)
        {
            weapon.OnAnimExit(atkCombo);
        }

    }
}