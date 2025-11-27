using Apis;
using EventData;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMoveComponent : UnitMoveComponent
{
    private Player player;

    public override void MoveOn()
    {
        base.MoveOn();
    }

    public override void Init(IMovable mover, Collider2D col)
    {
        base.Init(mover, col);
        player = mover as Player;
    }

    public override void MoveCCOn()
    {
        base.MoveCCOn();
    }

    public override void MoveCCOff()
    {
        base.MoveCCOff();
        player.SetState(EPlayerState.Idle);
    }

    public override void Stop()
    {
        base.Stop();
        player.animator.SetBool("IsMove", false);
    }

    public override void KnockBack(Vector2 src, KnockBackData knockBackData, UnityAction OnBegin,
        UnityAction OnEnd)
    {
        player.ExecuteEvent(EventType.OnHitReaction, null);

        /* invincible을 state 내부에서 처리로 변경 */
        // Guid guid = player.AddInvincibility();
        // IEnumerator InvincibleTimer()
        // {
        //     yield return new WaitForSeconds(player.hitInvincibleTime);
        //     player.RemoveInvincibility(guid);
        // }

        // Coroutine invincible = GameManager.instance.StartCoroutineWrapper(InvincibleTimer());

        base.KnockBack(src, knockBackData, () =>
        {
            /* 피격 시점에 State 변경 후 state 내부에서 knockback 호출 */
            // player.SetState(EPlayerState.Damaged);
            OnBegin?.Invoke();
        }, () =>
        {
            OnEnd?.Invoke();
            /* State 내부에서 자체 탈출 로직 설정(IdleOn 불필요)*/
            // player.IdleOn();
            // GameManager.instance.StopCoroutineWrapper(invincible);
            // player.RemoveInvincibility(guid);
        });
    }
}