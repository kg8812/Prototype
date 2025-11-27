using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimAtkMotionBehaviour : StateMachineBehaviour
{
    public enum AirOrGround
    {
        Air,
        Ground
    }

    public int atkCombo;

    public AirOrGround atkType;

    public bool isWeaponAtk;

    [HideIf("isWeaponAtk")] public bool isFinalAttack;

    private IEventUser actor;
    private Player p;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.transform.parent != null)
            actor = animator.transform.parent.GetComponentInChildren<IEventUser>(false);
        else
            actor = animator.transform.GetComponentInChildren<IEventUser>(false);

        var realActor = Utils.GetComponentInParentAndChild<Actor>(animator.gameObject);
        if (realActor != null)
        {
            animator.SetFloat("AtkSpeed", 1 + realActor.AtkSpeed / 100);
            if (realActor is Player player)
            {
                p = player;
                int max;
                player.weaponAtkInfo.atkCombo = atkCombo;
                player.weaponAtkInfo.airOrGround = atkType;
                var weapon = AttackItemManager.CurrentItem as Weapon;

                if (isWeaponAtk && weapon == null)
                {
                    Debug.LogError("무기가 할당되지 않았습니다.");
                    return;
                }

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
                            if (weapon.attackType == Weapon.AttackType.Collider)
                                weapon.SetGroundCollider(atkCombo - 1, player.attackColliders);

                        break;
                    case AirOrGround.Air:
                        max = animator.GetInteger("MaxAirAtk");

                        if (isWeaponAtk)
                            if (weapon.attackType == Weapon.AttackType.Collider)
                                weapon.SetAirCollider(atkCombo - 1, player.attackColliders);

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

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        actor.EventManager.ExecuteEvent(EventType.OnAttackEnd, new EventParameters(actor));

        if (actor is Player player && AttackItemManager.CurrentItem is Weapon weapon) weapon.OnAnimExit(atkCombo);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (p == null) return;
    }
}