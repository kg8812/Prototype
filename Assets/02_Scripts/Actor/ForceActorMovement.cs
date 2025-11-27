using Apis;
using UnityEngine;

public enum EDirection
{
    Up = 0b0001,
    Down = 0b0010,
    Right = 0b0100,
    Left = 0b1000
}

public class ForceActorMovement : ActorMovement
{
    private const float MoveCastDist = 0.2f;
    private const float SlopeChangeThreshold = 0.01f;
    private readonly IDirection _dirUser;
    private readonly IEventUser _eventUser;
    private readonly IMovable _mover;
    private readonly IMonoBehaviour _user;
    private readonly Collider2D collider;

    private readonly float _width;
    private readonly float _height;
    private readonly float castRadius;
    private float castDist;
    private Vector2 colliderOffset;

    #region force 이동 테스트용

    private readonly float maxSpeedRatio = 1f; // maxSpeedRaio / resistFactor * maxSpeed = 최종 종단 속도

    #endregion

    private float onChangeVel = 0;

    private float pastAngle;

    public ForceActorMovement(IMovable user, Collider2D _collider) : base(user, _collider)
    {
        _user = user;
        _mover = user;
        _dirUser = user.gameObject.GetComponent<IDirection>();
        _eventUser = user.gameObject.GetComponent<IEventUser>();

        collider = _collider;
        if (_collider is CapsuleCollider2D cap)
        {
            _width = cap.size.x;
            _height = cap.size.y;
        }
        else
        {
            _width = _collider == null || _collider.bounds.size.x == 0 ? 1 : _collider.bounds.size.x;
            _height = _collider == null || _collider.bounds.size.y == 0 ? 1.5f : _collider.bounds.size.y;
        }

        castRadius = _width / 2f;
        castDist = _height / 2f - castRadius;
        colliderOffset = _collider == null ? Vector2.zero : _collider.offset;
    }

    private new bool IsStick => _mover.ActorMovement.IsStick;
    private new bool IsSlope => _mover.ActorMovement.IsSlope;

    /// <summary>
    ///     Force 기반 이동 함수. Fixed Update 내에서 호출. 마찰 0일 때 상정
    /// </summary>
    /// <param name="resistFactor"> 저항 인자, 종단 속도까지 도달 시간 조절</param>
    /// <param name="maxSpeed"> 종단속도</param>
    public void Move(EActorDirection direction, float ratio, bool isFly = false, float resistFactor = 2.5f,
        float maxSpeed = 7f)
    {
        Vector2 forceDir;
        if (isFly)
        {
        }
        else
        {
            var dir = (int)direction;
            var force = Vector2.zero;
            Vector2 castOrigin = collider.bounds.center + _height * 0.4f * Vector3.down;

            /* 현재 이동 관련 속도 성분 계산 (공중의 경우 수평만, 경사에서는 경사 방향 속도도)*/
            var rad = pastAngle * Mathf.Deg2Rad;
            Vector2 proj = new(Mathf.Sin(rad) * (float)direction, Mathf.Cos(rad)); // pastAngle 방향 단위 벡터
            var currentVel = Vector2.Dot(_mover.Rb.linearVelocity, proj); // 성분 크기
            var currentAbsVel = Vector2.Dot(_mover.AbsVelocity, proj); // 현재 속도 크기
            // Debug.Log($"abs: {currentAbsVel}, rel: {currentVel}");

            Debug.DrawRay(collider.bounds.center + new Vector3(0, 0.1f, 0), proj, Color.yellow);

            /* 이동해야 하는 방향 벡터 계산 */
            RaycastHit2D hit;
            if (IsSlope)
                hit = Physics2D.Raycast(castOrigin + Vector2.right * (float)direction * castRadius, Vector2.down,
                    MoveCastDist * 2, LayerMasks.GroundAndPlatform);
            else
                hit = Physics2D.Raycast(castOrigin, Vector2.down, MoveCastDist, LayerMasks.GroundAndPlatform);

            if (hit.normal.sqrMagnitude > 0)
                forceDir = -Vector2.Perpendicular(hit.normal) * Mathf.Sign((float)direction);
            else
                forceDir = Vector2.right * (int)direction;

            /* 이전 각도와 비교 후 차이 나면 속도 방향 변경 */
            var currentAngle = Vector2.Angle(forceDir, Vector2.up);
            if (Mathf.Abs(pastAngle - currentAngle) > SlopeChangeThreshold)
            {
                if (IsStick)
                {
                    // 지상이면 이전 속도 성분을 현재 진행 방향으로
                    var newVelDir = Quaternion.Euler(0, 0, currentAngle * -dir) * Vector2.up;
                    _mover.Rb.linearVelocity = newVelDir.normalized * currentVel;
                }

                pastAngle = currentAngle;
            }

            Debug.DrawRay(collider.bounds.center, _mover.Rb.linearVelocity * 0.5f, Color.cyan);
            // Debug.Break();

            //TODO: moving obj 위에 있는 경우 abs velocity 계산 필요

            /* Fnet = F0 - bv => v(t) = v0(1-exp(-t/b)): 종단속도 적용 */
            var diff = Mathf.Sign(_mover.Rb.linearVelocity.x) * Mathf.Sign((float)_dirUser.Direction);
            force += 1 / resistFactor * (maxSpeedRatio * maxSpeed - currentVel * diff) * forceDir;

            Debug.DrawRay(collider.bounds.center, force * 0.1f, Color.red);
            _mover.Rb.AddForce(force);
        }
    }

    public new void Jump(float jumpforce)
    {
        _mover.Rb.linearVelocity = new Vector2(_mover.Rb.linearVelocity.x, 0);
        _mover.Rb.AddForce(jumpforce * Vector2.up, ForceMode2D.Impulse);
    }

    /// <summary>
    ///     공기 저항을 적용해서 종단 속도로 떨어지는 함수. Fixed Update 내에서 호출
    /// </summary>
    /// <param name="resistFactor"> 얼마나 저항을 강하게 받을지</param>
    /// <param name="endVel"> 종단속도</param>
    public void Drop(float resistFactor, float endVel)
    {
        if (_mover.Rb.linearVelocity.y > 0) return;

        // 종단 속도 도달 시 등속 낙하
        var resistForce = _mover.Rb.linearVelocity.y * (_mover.Rb.mass * Physics2D.gravity.y * GravityScale) / endVel;
        _mover.Rb.AddForce(resistForce * Vector2.up);
    }

    public void AirStop(float resistFactor)
    {
        if (_mover.Rb.linearVelocity.x == 0) return;

        _mover.Rb.AddForce(-_mover.Rb.linearVelocity.x * resistFactor * Vector2.right);
    }

    public void Friction(float frictionAmount)
    {
        if (_mover.Rb.linearVelocity.x == 0) return;

        var castCenter = (Vector2)collider.bounds.center + Vector2.down * _height * 0.49f;

        var amount = Mathf.Min(_mover.Rb.linearVelocity.magnitude, frictionAmount);

        amount *= Mathf.Sign(_mover.Rb.linearVelocity.x);

        var hit = Physics2D.Raycast(castCenter, Vector2.down, 0.1f, LayerMasks.GroundAndPlatform);

        var force = Vector2.Perpendicular(hit.normal) * amount;

        // Vector2 antiG;
        // if(Vector2.Angle(hit.normal, Vector2.up) > 0.05f){
        //     antiG = -_mover.Rb.mass * Physics2D.gravity;
        //     _mover.Rb.AddForce(antiG);
        // }

        _mover.Rb.AddForce(force, ForceMode2D.Impulse);
        Debug.DrawRay(collider.bounds.center, force, Color.blue);
    }

    public void Dash(float velocity, float angle)
    {
        // 속도 초기화
        _mover.Rb.linearVelocity = Vector2.zero;

        Vector2 forceDir = Quaternion.Euler(0, 0, angle) * Vector2.right;

        _mover.Rb.AddForce(forceDir * velocity, ForceMode2D.Impulse);
    }

    public void Dash(float velocity, Vector2 direction)
    {
        _mover.Rb.linearVelocity = Vector2.zero;

        _mover.Rb.AddForce(direction.normalized * velocity, ForceMode2D.Impulse);
    }

    public void Dash(float velocity, EDirection direction)
    {
        var forceDir = Vector2.zero;

        if ((direction & EDirection.Up) != 0) forceDir += Vector2.up;

        if ((direction & EDirection.Down) != 0) forceDir += Vector2.down;

        if ((direction & EDirection.Left) != 0) forceDir += Vector2.left;

        if ((direction & EDirection.Right) != 0) forceDir += Vector2.right;

        Dash(velocity, forceDir);
    }
}