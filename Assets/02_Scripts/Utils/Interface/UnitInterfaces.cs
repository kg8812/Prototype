using System;
using EventData;
using UnityEngine;
using UnityEngine.Events;

public enum EActorDirection
{
    Left = -1,
    Right = 1
}

namespace Apis
{
    public interface IAttackable : IMonoBehaviour
    {
        public float Atk { get; }
        public void AttackOn();
        public void AttackOff();
        public EventParameters Attack(EventParameters eventParameters);
    }

    public interface IMovable : IMonoBehaviour
    {
        public ActorMovement ActorMovement => MoveComponent?.ActorMovement;
        public UnitMoveComponent MoveComponent { get; }
        public float MoveSpeed { get; }

        /* World View에서 velocity (moving object 포함) */
        public Vector2 AbsVelocity
        {
            get
            {
                /* parent가 없는 경우 -> Rb 속도가 AbsVel */
                if (transform.parent == null)
                    return Rb.linearVelocity;

                /* movingObj 위에 있는 경우 moving Obj의 속도 합산 */
                if (MoveComponent.OnMovingObj != null)
                    return Rb.linearVelocity + MoveComponent.OnMovingObj.Velocity;

                /* 기타 예외 처리 필요하면 추가 */
                return Rb.linearVelocity;
            }
        }

        public Rigidbody2D Rb { get; }

        public void MoveOn()
        {
            MoveComponent?.MoveOn();
        }

        public void MoveOff()
        {
            MoveComponent?.MoveOff();
        }

        public void JumpOn()
        {
            MoveComponent?.JumpOn();
        }

        public void JumpOff()
        {
            MoveComponent?.JumpOff();
        }

        public void MoveCCOn()
        {
            MoveComponent?.MoveCCOn();
        }

        public void MoveCCOff()
        {
            MoveComponent?.MoveCCOff();
        }

        public void Stop()
        {
            MoveComponent?.Stop();
        }

        /// <summary>
        ///     넉백 함수, src에서 멀어지는 방향으로 넉백 작용
        /// </summary>
        /// <param name="src">멀어질 기준의 위치 벡터 </param>
        /// <param name="knockBackForce">넉백 파워 </param>
        /// <param name="knockBackTime">이 시간후에 OnEnd가 실행됨, 다른 효과는 없음</param>
        /// <param name="knockBackAngle">넉백 각도</param>
        /// <param name="OnBegin">넉백 실행 전 실행될 이벤트(컨트롤 막기 등)</param>
        /// <param name="OnEnd">넉백 실행 후 실행될 이벤트 </param>
        public void KnockBack(Vector2 src, KnockBackData knockBackData,
            UnityAction OnBegin, UnityAction OnEnd)
        {
            MoveComponent?.KnockBack(src, knockBackData, OnBegin, OnEnd);
        }
    }

    public interface IDirection : IMonoBehaviour
    {
        public EActorDirection Direction { get; }
        public int DirectionScale { get; }
        public void SetDirection(EActorDirection dir);
    }

    public interface IOnHit : IMonoBehaviour
    {
        public float MaxHp { get; }
        public float CurHp { get; set; }
        public bool IsDead { get; }
        public bool HitImmune { get; }
        public bool IsAffectedByCC { get; }
        public bool IsInvincible { get; }
        public int Exp { get; }
        float OnHit(EventParameters parameters);
        public Guid AddInvincibility();
        public void RemoveInvincibility(Guid guid);
        public Guid AddHitImmunity();
        public void RemoveHitImmunity(Guid guid);
    }

    public interface IOnHitReaction
    {
        KnockBackData GetKnockBackData(EventParameters parameters);
    }


    public interface IDashUser : IMonoBehaviour
    {
        public bool IsDash { get; set; }
        public void SetDash(Player.IPlayerDash dash);
        public void SetDashToNormal();
        public void DashOn();
        public void DashOff();
    }

    public interface IStatUser : IMonoBehaviour
    {
        public StatManager StatManager { get; }
    }

    public interface IBarrierUser : IMonoBehaviour
    {
        public BarrierCalculator BarrierCalculator { get; }
    }
}