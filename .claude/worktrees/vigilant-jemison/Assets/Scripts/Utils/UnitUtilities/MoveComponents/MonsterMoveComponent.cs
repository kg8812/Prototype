using System.Collections;
using Apis;
using EventData;
using UnityEngine;
using UnityEngine.Events;

public class MonsterMoveComponent : UnitMoveComponent
{
    private Coroutine KnockBackCoroutine;
    private Monster monster;

    public override void Init(IMovable mover, Collider2D col)
    {
        base.Init(mover, col);

        monster = mover as Monster;
    }

    public override void KnockBack(Vector2 src, KnockBackData knockBackData, UnityAction OnBegin,
        UnityAction OnEnd)
    {
        if (monster.canKnockBacked)
        {
            Debug.Log("KnockBack2");

            if (KnockBackCoroutine != null) StopCoroutine(KnockBackCoroutine);
            OnBegin?.Invoke();
            ActorMovement.KnockBack2(src, knockBackData);
            // 몬스터의 경우 Time이 아닌 vel.x, y가 최소가 근접할때까지 반복
            KnockBackCoroutine = StartCoroutine(CheckKnockBackEnd(OnEnd));
        }
    }

    private IEnumerator CheckKnockBackEnd(UnityAction onEnd, float minXY = 0.01f)
    {
        while (true)
        {
            yield return null;
            // magnitude가 맞긴하지만 어차피 적은거 판정이라 최적화 면에서 x, y 따로 계산
            if (Mathf.Abs(monster.Rb.linearVelocity.x) <= minXY && Mathf.Abs(monster.Rb.linearVelocity.y) <= minXY)
            {
                onEnd?.Invoke();
                break;
            }
        }
    }

    public override void MoveCCOn()
    {
        base.MoveCCOn();
    }

    public override void MoveCCOff()
    {
        base.MoveCCOff();
    }
}