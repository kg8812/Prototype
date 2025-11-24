using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis;
using DG.Tweening;
using EventData;
using UnityEngine;
using UnityEngine.Events;

public class UnitMoveComponent : MonoBehaviour
{
    public ActorMovement ActorMovement { get; private set; }
    private ForceActorMovement _forceActorMovement;
    public ForceActorMovement ForceActorMovement => _forceActorMovement;
    public bool isJump { get; set; }
    public bool ableJump { get; set; }
    public bool ableMove { get; set; }

    private IImmunity _immunity;
    protected ImmunityController immunityController => _immunity?.ImmunityController;
    public virtual void Init(IMovable mover, Collider2D col)
    {
        ActorMovement = new(mover, col);
        _immunity = mover.gameObject.GetComponent<IImmunity>();
        _forceActorMovement = new(mover, col);
    }

    public MovingObj OnMovingObj { get; set; } // movingObj 위에 있는지 여부

    /// <summary>
    /// 넉백 함수, src에서 멀어지는 방향으로 넉백 작용
    /// </summary>
    /// <param name="src">멀어질 기준의 위치 벡터 </param>
    /// <param name="knockBackForce">넉백 파워 </param>
    /// <param name="knockBackTime">이 시간후에 OnEnd가 실행됨, 다른 효과는 없음</param>
    /// <param name="knockBackAngle">넉백 각도</param>
    /// <param name="OnBegin">넉백 실행 전 실행될 이벤트(컨트롤 막기 등)</param>
    /// <param name="OnEnd">넉백 실행 후 실행될 이벤트 </param>
    public virtual void KnockBack(Vector2 src, KnockBackData knockBackData,
        UnityAction OnBegin, UnityAction OnEnd)
    {
        // TODO: 공격 불가, 이동불가, 공격 캔슬
        OnBegin?.Invoke();
        // ActorMovement.KnockBack(src, knockBackForce, knockBackAngle);
        ActorMovement.KnockBack2(src, knockBackData);
        Sequence seq = DOTween.Sequence();
        seq.SetDelay(knockBackData.knockBackTime);
        seq.AppendCallback(OnEnd.Invoke);
    }

    private void FixedUpdate()
    {
        ActorMovement?.Update();
    }

    public virtual void MoveOn()
    {
        ableMove = true;
    }

    public virtual void MoveOff()
    {
        ableMove = false;
    }

    public virtual void JumpOn()
    {
        ableJump = true;
    }

    public virtual void JumpOff()
    {
        ableJump = false;
    }

    public virtual void MoveCCOn()
    {
        MoveOff();
        JumpOff();
        ActorMovement.Stop();
    }

    public virtual void MoveCCOff()
    {
        MoveOn();
        JumpOn();
    }

    public virtual void Stop()
    {
        ActorMovement.Stop();
    }
}