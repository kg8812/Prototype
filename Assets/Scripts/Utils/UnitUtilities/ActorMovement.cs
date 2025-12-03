using System.Collections.Generic;
using chamwhy;
using UnityEngine;
using DG.Tweening;
using System;
using Apis;
using Default;
using Sirenix.Utilities;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using EventData;

// using System.Numerics;

[Serializable]
[FoldoutGroup("기획쪽 수정 변수들")]
[TabGroup("기획쪽 수정 변수들/group1", "충돌 관련 스탯")]
[HideLabel]
public class ActorMovement // 유닛 이동관련 기능 클래스 (이동, 점프, 바닥 체크 등)
{
    [LabelText("중력 크기")] [SerializeField] private float defaultGravityScale = 2f;
    private float _GravityScale;

    protected const float slopeAngleThreshold = 60f;
    public float SlopeAngleThreshold => slopeAngleThreshold;
    
    public float GravityScale => _GravityScale;

    private readonly IMonoBehaviour _user;
    private readonly IMovable _mover;
    private readonly IDirection _dirUser;
    private readonly IEventUser _eventUser;

    readonly float _height;
    readonly float _width;

    public float Width => _width;
    public float Height => _height;
    [HideInInspector] public Vector2 dirVec;
    [HideInInspector] public float dragFactor = 1; // Player-Enemy 충돌 시 저항 값, range: (0, 1]


    private Vector2 _dir;
    private Vector2 _dashDst;
    private float castRadius;
    private float castDist;
    private Collider2D collider;
    private Vector2 colliderOffset;

    Tweener tweener;
    public Tweener Tweener => tweener;

    public float AirHoldingTime { get; private set; } // 공중 체공시간

    public ActorMovement(IMovable user,Collider2D _collider)
    {
        _user = user;
        _mover = user;
        _dirUser = user.gameObject.GetComponent<IDirection>();
        _eventUser = user.gameObject.GetComponent<IEventUser>();
        _GravityScale = defaultGravityScale;

        collider = _collider;
        if(collider is CapsuleCollider2D cap)
        {
            var capsule = cap;
            _width = capsule.size.x;
            _height = capsule.size.y;
        }
        else
        {
            _width = collider == null || collider.bounds.size.x == 0 ? 1 : collider.bounds.size.x;
            _height = collider == null || collider.bounds.size.y == 0 ? 1.5f : collider.bounds.size.y;
        }
        castRadius = _width / 2f;
        castDist = _height / 2f - castRadius;
        colliderOffset = collider == null ? Vector2.zero : collider.offset;

        isStick = UpdateIsStick();
        isSlope = UpdateIsSlope();
    }

    public void SetGravityToZero()
    {
        _mover.Rb.gravityScale = 0;
    }

    public void SetGravityScale(float value)
    {
        // _mover.Rb.gravityScale = value;
        _GravityScale = value;
    }
    public void ResetGravityScale()
    {
        _GravityScale = defaultGravityScale;
    }

    public void SetGravity()
    {
        _mover.Rb.gravityScale = GravityScale;
    }

    public void ResetGravity()
    {
        _mover.Rb.gravityScale = defaultGravityScale;
    }

    private bool isStick = true;
    public bool IsStick => isStick;

    public bool UpdateIsStick()
    {
        Vector2 castCenter = (Vector2)collider.bounds.center + Vector2.down * castDist;
        
        // 발 밑 우선 체크
        RaycastHit2D under = Physics2D.Raycast(
            castCenter + castRadius * Vector2.down,
            Vector2.down,
            0.03f,
            LayerMasks.GroundAndPlatform
            );
        
        Debug.DrawLine(under.point, under.point + under.normal * 0.5f, Color.red);
        
        /* 발 밑이 붙어있음 (boxcast 절약용으로 이용)*/
        if(under.normal.sqrMagnitude > 0 && Vector2.Angle(under.normal, Vector2.up) < slopeAngleThreshold) 
            return true;

        RaycastHit2D[] rays = Physics2D.BoxCastAll(
            castCenter + castRadius * Vector2.down,
            new Vector2(_width, castRadius * 0.2f), 
            0, 
            Vector2.down, 
            0.03f, 
            LayerMasks.GroundAndPlatform
        );

        /* 발 밑 boxcast 결과가 없음 */
        if (rays.IsNullOrEmpty())
            return false;

        foreach (RaycastHit2D ray in rays)
        {
            // Debug.Log(ray.point);
            Debug.DrawLine(ray.point, ray.point + ray.normal * 0.5f, Color.blue);
            /* collider가 찍히고, normal 위 방향인 경우만 판정 */
            if (ray && ray.normal.sqrMagnitude > 0 && ray.normal.y > 0)
            {
                /* boxcollider 내부에서 stick 판정 해결 */
                if(ray.collider.gameObject.layer == LayerMasks.Platform 
                && ray.collider.OverlapPoint(ray.point + new Vector2(0, 0.01f))) 
                    continue;
                
                float angle = Vector2.Angle(ray.normal, Vector2.up);
                
                /* 경사로 취급하는 각도보다 높은 경우(=벽) 패스*/
                if (angle > slopeAngleThreshold) continue;

                return true;
            }
        }
        return false;
    }

    public void Update()
    {
        /* 매 fixedUpdate당 한 번만 계산 후 캐싱 값 이용 -> UnitMoveComponent 실행 우선 순위 높임 */
        isStick = UpdateIsStick();
        isSlope = UpdateIsSlope();
        // Debug.Log(collider.transform.parent.name + " " + isStick + " " + isSlope);
        if (IsStick)
        {
            AirHoldingTime = 0;
        }
        else
        {
            AirHoldingTime += Time.fixedDeltaTime;
        }
    }

    private bool isSlope = false;
    public bool IsSlope => isSlope;

    private const float SlopeCheckDistance = 0.1f;
    private bool UpdateIsSlope()
    {
        Vector2 castCenter = (Vector2)collider.bounds.center + Vector2.down * _height * 0.49f;

        RaycastHit2D center = Physics2D.Raycast(castCenter, Vector2.down, SlopeCheckDistance, LayerMasks.GroundAndPlatform);
        
        Debug.DrawRay(castCenter, Vector2.down * SlopeCheckDistance, Color.green);
        // 공중에 떠있는가
        if(center.normal.sqrMagnitude <= 0) return false;

        // 평지에 콜라이더 중앙이 있는가
        float angleCenter = Vector2.Angle(center.normal, Vector2.up);
        if(angleCenter < 0.05f || angleCenter >= slopeAngleThreshold) return false;

        // 전방에 경사가 있는가(바로 아래보다 전방 경사 우선 체크, 경사 -> 평지 전환 경우 고려)
        RaycastHit2D front = Physics2D.Raycast(castCenter, Vector2.right * (float)_dirUser.Direction, _width * 0.5f, LayerMasks.GroundAndPlatform);
        if(front.normal.sqrMagnitude > 0)
        {
            float angleFront = Vector2.Angle(front.normal, Vector2.up);
            if(angleFront >= 0.05f && angleFront <slopeAngleThreshold) return true;
        }

        // 현재 서있는 곳이 경사인가
        if(angleCenter >= 0.05f && angleCenter <slopeAngleThreshold) return true;

        RaycastHit2D back = Physics2D.Raycast(castCenter, -Vector2.right * (float)_dirUser.Direction, _width * 0.5f, LayerMasks.GroundAndPlatform);
        if(back.normal.sqrMagnitude > 0)
        {
            float angleBack = Vector2.Angle(back.normal, Vector2.up);
            if(angleBack >= 0.05f && angleBack <slopeAngleThreshold) return true;
        }

        return false;
    } 

    public T GetMovingObj<T>() where T : MovingObj
    {
        Vector2 castCenter = (Vector2)collider.bounds.center + Vector2.down * castDist;
        RaycastHit2D[] rays = Physics2D.BoxCastAll(castCenter + 0.9f * castRadius * Vector2.down,
            new Vector2(_width, castRadius * 0.2f), 0, Vector2.down, 0.03f, LayerMasks.MapAndPlatform);
        

        if (rays.IsNullOrEmpty()) return default;

        foreach (RaycastHit2D ray in rays)
        {
            if(!ray.collider.TryGetComponent<T>(out var platform)) return default;

            return platform;
        }
        return default;
    }

    /* 현재 서있는 곳의 기울기 */
    public float GetSlope()
    {
        float searchDepth = 0.3f;

        var position = _mover.transform.position;
        RaycastHit2D ray = Physics2D.Raycast(position, Vector2.down, searchDepth,
            LayerMasks.GroundAndPlatform);

        if (ray.normal.sqrMagnitude > 0)
        {
            //get slope
            return Vector2.Angle(ray.normal, Vector2.up);
        }

        return 0;
    }

    public Vector2 GetSlopeVetor()
    {
        float searchDepth = 0.3f;

        var position = _mover.transform.position;
        RaycastHit2D ray = Physics2D.Raycast(position, Vector2.down, searchDepth,
            LayerMasks.GroundAndPlatform);
        int scale = _dirUser?.DirectionScale ?? 1;
        
        if(ray.normal.sqrMagnitude > 0)
            return (-1) * scale * Vector2.Perpendicular(ray.normal);

        return Vector2.right * scale;
    }

    public Vector2 DownPoint(int rl)
    {
        return Vector2.zero;
    }

    public void Jump(float jumpVelocity)
    {
        _mover.Rb.linearVelocity = new Vector2(_mover.Rb.linearVelocity.x, jumpVelocity);
        _mover.Rb.gravityScale = GravityScale;
    }

    public void StopWithFall()
    {
        _mover.Rb.linearVelocity = new Vector2(0, _mover.Rb.linearVelocity.y);
    }

    public void Stop()
    {
        _mover.Rb.DOKill();
        _mover.Rb.linearVelocity = Vector2.zero;
    }

    public void Move(EActorDirection direction, float ratio, bool isFly = false)
    {
        //if(CheckWall()) return;
        Move((int)direction, ratio, isFly);
    }

    private EventParameters _moveParameters;

    private void Move(int rl, float ratio, bool isFly = false)
    {
        // lerp를 vel기준에서 actor의 velocity 기준으로 변경했습니다.
        // 넉백같이 velocity를 순간적으로 변경하는 경우 때문임
        float vel = Mathf.Lerp(_mover.Rb.linearVelocity.x, _mover.MoveSpeed * rl * 0.01f * ratio, Time.deltaTime * 5);
        if (isFly)
        {
            dirVec = new Vector2(1.0f, 0) * vel;
        }
        else
        {
            bool isStick = IsStick;
            if (!isStick)
            {
                _mover.Rb.gravityScale = GravityScale;
            }
            else
            {
                _mover.Rb.gravityScale = 0;
            }

            // MoveSpeed 100 = 1unit/s
        
            if (isStick)
            {
                // Vector2 actorPos = _user.transform.position;
                RaycastHit2D rayHit = Physics2D.Raycast(
                    _mover.Position + _height * 0.25f * Vector3.down + new Vector3(_width / 1.5f * rl, 0),
                    Vector2.down, _height * 0.75f, LayerMasks.GroundAndPlatform);
                
                RaycastHit2D actorPosHit = Physics2D.Raycast(
                    _mover.Position + _height * 0.25f * Vector3.down,
                    Vector2.down, _height * 0.75f, LayerMasks.GroundAndPlatform);

                Debug.DrawRay(_mover.Position + _height * 0.25f * Vector3.down + new Vector3(_width / 1.5f * rl, 0),
                    new Vector2(0, -0.75f), Color.blue);
                // Debug.DrawRay(actorPos, rayHit.point - actorPos, Color.red);
                Debug.DrawRay(_mover.Position, rayHit.point - actorPosHit.point, Color.red);

                if (rayHit.collider != null && actorPosHit.collider != null)
                {
                    /* 경사 있음 */
                    Vector2 dir = (rayHit.point - actorPosHit.point).normalized;

                    float angle = Vector2.Angle(Vector2.right * rl, dir);
                    if (angle is < 45 and > 3)
                    {
                        float crl = _mover.Rb.linearVelocity.x > 0 ? 1 : -1;
                        vel = Mathf.Lerp(_mover.Rb.linearVelocity.magnitude * crl, _mover.MoveSpeed * rl * 0.01f * ratio, Time.deltaTime * 5);
                        dirVec = dir * Mathf.Abs(vel);
                    }
                    else
                    {
                        // 일정 경사 이상이면 직선
                        dirVec = new Vector2(1.0f, 0) * vel;
                    }
                }
                else
                {
                    /* 경사 없음 */
                    dirVec = new Vector2(1.0f, 0) * vel;
                }
            }
            else
            {
                dirVec = new Vector2(1.0f, 0) * vel;
            }

            // TODO: 어떤 의미에서의 예외처리인가? 
            //-> 자잘한 y방향 워블 무시용인거 같은데 없으면 어떻게 되는지 모르겠음
            if (isStick)
            {
                _mover.Rb.linearVelocity = dirVec * dragFactor;
            }
            else
            {
                _mover.Rb.linearVelocity = new Vector2(vel, _mover.Rb.linearVelocity.y);
            }
        }
        _moveParameters ??= new EventParameters(_eventUser);
        _eventUser?.EventManager.ExecuteEvent(EventType.OnMove, _moveParameters);
    }
    
    public void StepMove()
    {
        Vector2 actorPos = _user.transform.position;
        int scale =_dirUser?.DirectionScale ?? 1;
        RaycastHit2D rayHit = Physics2D.Raycast(
            _mover.Position + _height * 0.25f * Vector3.down + new Vector3(_width / 1.5f * scale, 0),
            Vector2.down, 0.75f, LayerMasks.GroundAndPlatform);

        if (rayHit.collider != null)
        {
            Vector2 dir = (rayHit.point - actorPos).normalized;
            tweener = _mover.Rb.transform.DOMove(_mover.transform.position + (Vector3)dir * 0.3f, 0.1f).SetAutoKill(true);
        }
    }

    public void Crouch()
    {
        /* 타격 범위 등 조절(CapsuleCollider only) */
        if(collider is not CapsuleCollider2D playerCollider) return;

        if (playerCollider == null) return;
        
        playerCollider.offset = new Vector2(colliderOffset.x, colliderOffset.y - 0.1f * _height);
        playerCollider.size = new Vector2(_width, _height * 0.8f);

        castDist = _height * 0.8f / 2f - castRadius;
    }

    public void StandUp()
    {
        if(collider is not CapsuleCollider2D playerCollider) return;

        if (playerCollider == null) return;
        playerCollider.size = new Vector2(_width, _height);
        playerCollider.offset = colliderOffset;

        castDist = _height / 2f - castRadius;
    }

    // from에서 현재 액터 방향으로 force만큼 넉백
    public void KnockBack(Vector2 from, float force, float angle = 20)
    {
        float xdir = _mover.Position.x - from.x > 0 ? 1 : -1;
        Vector2 dir = Vector2.right * xdir;
        dir = Quaternion.AngleAxis(angle, Vector3.forward * xdir) * dir;
        Debug.DrawRay(_mover.Position, dir * 10, Color.red);
        _mover.Rb.gravityScale = GravityScale;
        _mover.Rb.AddForce(dir * force, ForceMode2D.Impulse);
    }

    public void KnockBack2(Vector2 from, KnockBackData knockBackData)
    {
        
        Vector2 force = Vector2.zero;
        switch (knockBackData.directionType)
        {
            case KnockBackData.DirectionType.AttackerRelative:
            case KnockBackData.DirectionType.AktObjRelative:
                force = ((Vector2)_mover.Position - from).normalized;
                break;
            
            case KnockBackData.DirectionType.AbsoluteAngle:
                float xdir = _mover.Position.x - from.x > 0 ? 1 : -1;
                force = Vector2.right * xdir;
                force = Quaternion.AngleAxis(knockBackData.knockBackAngle, Vector3.forward * xdir) * force;
                break;
        }

        switch (knockBackData.symmetryType)
        {
            case KnockBackData.SymmetryType.Vertical:
                force = new Vector2(-force.x, force.y);
                break;
            case KnockBackData.SymmetryType.Horizontal:
                force = new Vector2(force.x, -force.y);
                break;
            case KnockBackData.SymmetryType.Point:
                force = new Vector2(-force.x, -force.y);
                break;
        }
        
        

        force *= knockBackData.knockBackForce;
        
        Debug.DrawRay(_mover.Position, force, Color.red);
        
        _mover.Rb.gravityScale = GravityScale;
        // 넉백 자체에 velocity 초기화 추가
        _mover.Rb.linearVelocity = Vector2.zero;
        _mover.Rb.AddForce(force, ForceMode2D.Impulse);
    }

    // TODO: 전반적인 casting 방식을 ray 말고 collider 맞춰서 boxcast나 spherecast로 변경도 생각중
    // TODO: Cliff 체크에는 ray 3개면 될것 같은데 왜 최소 3번, 최대 12번 하는가?
    public bool CheckCliff()
    {
        // TODO: CheckCliff는 update 함수라 raycastHit2Ds를 밖으로 빼는 것을 제안 (수정함)
        // 몬스터 현재 방향에 대해 cliff 판단 여부 반환
        float scale =  _dirUser?.DirectionScale ?? 1;
        float searchDepth = 0.1f;
        Vector2 castCenter = (Vector2)collider.bounds.center + _height * 0.49f * Vector2.down;
        float angle = GetSlope();

        List<RaycastHit2D> hits = new() {
            // Physics2D.Raycast(castCenter, Vector2.down, searchDepth, LayerMasks.MapAndPlatform),
            Physics2D.Raycast(castCenter + _width * 0.5f * scale * Vector2.right, Vector2.down, searchDepth + Mathf.Tan(angle) * _width * 0.5f, LayerMasks.GroundAndPlatform),
            Physics2D.Raycast(castCenter - _width * 0.5f * scale * Vector2.right, Vector2.down, searchDepth + Mathf.Tan(angle) * _width * 0.5f, LayerMasks.GroundAndPlatform)
        };
        
        Debug.DrawRay(castCenter + _width * 0.5f * scale * Vector2.right,
            Vector2.down * (searchDepth + Mathf.Tan(angle) * _width * 0.5f), Color.cyan, 0.1f);
        if(hits.Count == 0) return false;
        
        

        // bool mid = hits[0].normal.sqrMagnitude > 0;
        bool front = hits[0].normal.sqrMagnitude > 0;
        bool back = hits[1].normal.sqrMagnitude > 0;
        
        if((back && !front)) return true;

        return false;
    }

    public bool CheckClimb()
    {
        int scale = _dirUser?.DirectionScale ?? 1;
        float searchWidth = 0.03f;

        Vector2 castCenter = (Vector2)collider.bounds.center + Vector2.right * (scale * _width * 0.5f);
        RaycastHit2D[] rays = Physics2D.BoxCastAll(castCenter, new Vector2(0.01f, _height * 0.98f), 0,
            Vector2.right * scale, searchWidth, LayerMasks.GroundAndPlatform);

        foreach (RaycastHit2D ray in rays)
            if (ray.normal.sqrMagnitude > 0
                && Vector2.Angle(Vector2.up, ray.normal) > slopeAngleThreshold)
                return true;
        return false;
    }
    
    // 벽체크 Ray 탐색 깊이
    private const float checkWallSerchDepth = 0.3f;
    // 벽체크 두 레이 최대 차이
    private const float checkWallUpDownMaxDiff = 0.01f;

    public bool CheckWall2()
    {
        int scale =  _dirUser?.DirectionScale ?? 1;

        return CheckWall(scale, checkWallSerchDepth);
    }
    
    public bool CheckWall(float scale, float depth)
    {
        Vector2 castCenter = (Vector2)collider.bounds.center + Vector2.down * castDist;
        
        Vector2 upRay = castCenter + new Vector2(scale * _width * 0.5f, 0);
        Vector2 downRay = castCenter + new Vector2(scale * _width * 0.5f, -castRadius * 0.4f);
        
        Debug.DrawRay(upRay, Vector2.right * (scale * depth), Color.magenta, 0.2f);
        Debug.DrawRay(downRay, Vector2.right * (scale * depth), Color.magenta, 0.2f);

        float upDist = Physics2D.Raycast(upRay, Vector2.right * scale, depth, LayerMasks.Wall).distance;
        RaycastHit2D downRaycastHit2D = Physics2D.Raycast(downRay, Vector2.right * scale, depth, LayerMasks.Wall);
        
        // 아래 레이가 벽에 닿지 않았다? => 벽이 아님.
        if (downRaycastHit2D.normal.sqrMagnitude <= 0)
        {
            return false;
        }

        if (Mathf.Abs(upDist - downRaycastHit2D.distance) > checkWallUpDownMaxDiff)
        {
            return false;
        }
        else
        {
            if (downRaycastHit2D.collider.gameObject.TryGetComponent(out ICollisionEvents col))
            {
                col.OnCollide(_user.gameObject);
            }
            return true;
        }
    }

    public bool CheckMovable()
    {   
        Vector2 castCenter = (Vector2)collider.bounds.center + Vector2.down * castDist;

        float scale = _dirUser?.DirectionScale ?? 1;
        float dist = _width * 0.5f + 0.05f;

        RaycastHit2D hit = Physics2D.Raycast(castCenter, Vector2.right * scale, dist, LayerMasks.GroundAndPlatform);
        Debug.DrawRay(castCenter, Vector2.right * (scale * dist), Color.magenta);

        if(hit.normal.sqrMagnitude <= 0)
            return true;

        if(Vector2.Angle(hit.normal, Vector2.up) < slopeAngleThreshold)
            return true;

        return false;
    }
    
    float prevangle;

    private UnityEvent<DashInfo> _whenSlope;
    public UnityEvent<DashInfo> WhenSlope => _whenSlope ??= new();

    public struct DashInfo
    {
        public Vector2 startPos; // 시작 지점
        public Vector2 endPos; // 끝 점
        public float duration; // 지속 시간
        public float angle; // 각도
    }
    
    float _timer;

    public Tweener DashInSpeed(float speed, float distance, bool isBackDash)
    {
        return DashTemp(distance / speed, distance, isBackDash);
    }

    public Tweener DashInDirection(float time, float distance, Vector2 direction)
    {
        prevangle = 0;
        _timer = time;

        Vector2 position = _user.transform.position;
        _dashDst = position + direction*distance;

        var _dashTweener = _mover.Rb.DOMove(_dashDst, distance/time).SetUpdate(UpdateType.Fixed).SetSpeedBased();
        _dashTweener.SetEase(Ease.OutSine);
        // _dashTweener.OnUpdate(() => DashOnUpdateCallback(_dashTweener, _dashDst));
        
        return _dashTweener;
    }
    public Tweener DashTemp(float time, float distance, bool isBackDash, DG.Tweening.Ease graph = DG.Tweening.Ease.OutSine)
    {
        int scale =  _dirUser?.DirectionScale ?? 1;
        float temp = distance * scale;
        prevangle = 0;
        _timer = time;

        if (isBackDash) temp *= -1;
        Vector2 position = _user.transform.position;
        float x = position.x + temp;
        var _dashDst = new Vector2(x, position.y);

        var _dashTweener = _mover.Rb.DOMove(_dashDst, distance/time).SetUpdate(UpdateType.Fixed).SetSpeedBased().SetAutoKill(true);
        _dashTweener.SetEase(graph);
        _dashTweener.OnUpdate(() => DashOnUpdateCallback(_dashTweener, _dashDst));
        
        return _dashTweener;
    }
    void DashOnUpdateCallback(Tweener tweener, Vector2 dashDst)
    {
        _timer -= Time.fixedDeltaTime;
        float scale = _dirUser.DirectionScale;
        Vector2 castCenter = (Vector2)collider.bounds.center + Vector2.down * castDist;
        RaycastHit2D forwardHit = Physics2D.Raycast(
            castCenter + castRadius * scale * Vector2.right, 
            Vector2.down, castRadius + 0.2f, LayerMasks.GroundAndPlatform);

        if (tweener == null || (!IsStick && forwardHit.normal.sqrMagnitude == 0)) return;
        if (CheckClimb())
        {
            tweener.Kill();
            return;
        }

        RaycastHit2D rayHit = Physics2D.Raycast(
            // position + new Vector3(0, -_height * 0.25f, 0), // 현재 발 밑 캐스팅
            castCenter,
            Vector2.down, castRadius + 0.2f, LayerMasks.GroundAndPlatform);
        Debug.DrawRay(castCenter, Vector2.down * (castRadius + 0.2f), Color.cyan);

        if (rayHit.collider == null)  // 캐스팅 된 지면이 없는 경우 패스
            return;

        
        Vector2 dir = -1 * scale * Vector2.Perpendicular(rayHit.normal).normalized; // 변경된 대쉬 방향
        float angle = Vector2.Angle(Vector2.right * scale, dir); // 경사 각도

        if (angle < slopeAngleThreshold && Mathf.Abs(angle - prevangle) > 0.1f) // 경사 각도 미만에서만 대쉬 탈 수 있고 이전 각도와 달라진 경우에 한해서만 경로 변경 진행
        {
            // 남은 거리 계산 (X축 기준)
            float currentX = _mover.transform.position.x;
            float remainingX = dashDst.x - currentX;
            
            // 이미 목표 지점을 지났거나 방향이 역전된 경우 패스
            if ((scale > 0 && remainingX <= 0) || (scale < 0 && remainingX >= 0)) return;
            
            float factor = 1 / Mathf.Cos(Mathf.Deg2Rad * angle); // 경사에서 길이 보정 인자

            // 현재 위치에서 남은 거리만큼 경사면 방향으로 뻗은 새 목적지
            Vector2 newDst = (Vector2)_mover.transform.position + (Mathf.Abs(remainingX) * factor * dir);
            
            tweener.ChangeEndValue(newDst, true);
            
            DashInfo info = new ()
            {
                angle = angle,
                endPos = newDst,
                startPos = _mover.Rb.position,
                duration = _timer > 0 ? _timer : 0
            };
            prevangle = angle;
            
            WhenSlope.Invoke(info);
        }
    }

    // 경사 차이 살짝 발생했을 때 무시하고 대쉬 진행
    private void EdgeCorrection()
    {
        Vector2 dir = Vector2.right * (float)_dirUser.Direction;
        Vector2 castOrigin = (Vector2)collider.bounds.center + _width * 0.55f * dir + _height * 0.5f * Vector2.up;

        if(_user is not Actor actor) return;

        RaycastHit2D hit = Physics2D.Raycast(castOrigin, Vector2.down, _height, LayerMasks.GroundAndPlatform);
        Debug.DrawRay(castOrigin, Vector2.down * _height, Color.red);

        if(hit.normal.sqrMagnitude <= 0 && Vector2.Angle(dir, Vector2.up) < slopeAngleThreshold) return;

        float diff = hit.point.y - _mover.Rb.position.y;

        if(diff <= 0 || diff > actor.MaxEdgeModifier) return;

        _mover.Rb.position += Vector2.up * (diff + 0.05f); 
    }
    
    public (Tween, Tween) DoJumpTween(float time, float height, float xDistance,float yDistance,bool isBack)
    {
        float scale = _dirUser?.DirectionScale ?? 1;
        float tempX = xDistance * scale;
        float tempY = yDistance;
        
        if (isBack) tempX *= -1;
        Vector2 position = _user.transform.position;

        
        float x = position.x + tempX;
        float y = position.y + tempY;
        Vector2 pos = new Vector2(x, y);

        var values = yDistance > 0 ? _mover.Rb.DOJumpUp(pos, height, time) : _mover.Rb.DOJumpDown(pos, height, time);
        
        values.Item1.SetUpdate(UpdateType.Fixed);
        values.Item2.SetUpdate(UpdateType.Fixed);
        
        return values;
    }
    public (Tween, Tween) DoJumpTween(float time, float height, float distance, bool isBack)
    {
        float scale = _dirUser?.DirectionScale ?? 1;
        float temp = distance * scale;

        if (isBack) temp *= -1;
        Vector2 position = _user.transform.position;

        float x = position.x + temp;
        Vector2 pos = new Vector2(x, position.y);

        var values = _mover.Rb.DoJumpTween(pos, height, time);
        
        values.Item1.SetUpdate(UpdateType.Fixed);
        values.Item2.SetUpdate(UpdateType.Fixed);
        return values;
    }

    public bool TryGetFloorPos(out Vector2 floorPos)
    {
        if (Utils.GetLowestPointByRay(_mover.Position, LayerMasks.GroundAndPlatform, out var value))
        {
            floorPos = value;
            return true;
        }

        floorPos = Vector2.zero;
        return false;
    }
}