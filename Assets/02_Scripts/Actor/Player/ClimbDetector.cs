using System.Collections;
using UnityEngine;
using Apis;
using Default;
using System.Collections.Generic;
using System.Threading;

public enum EClimbType
{
    NONE, LEDGEHIGH, LEDGELOW, PLATFORMHIGH, PLATFORMLOW,
}

public class ClimbDetector : MonoBehaviour
{
    BoxCollider2D col;
    private Player _player;
    private float _playRatio;

    Player Player
    {
        get
        {
            if (_player == null)
            {
                _player = Utils.GetComponentInParentAndChild<Player>(gameObject);
            }

            return _player;
        }
    }



    private float _height;
    private float _width;

    private LayerMask mapLayerMask;
    private const float searchWidth = 0.2f;

    private Vector2 climbEndPos;
    private HashSet<Collider2D> enteredColliders = new();

    // private Rigidbody2D RB;

    private const float climbDepth = 1;

    private void Awake()
    {
        col = gameObject.GetComponent<BoxCollider2D>();
        _height = col.size.y;
        _width = col.size.x;
        // RB = GetComponent<Rigidbody2D>();
        // rect.sizeDelta = new Vector2(_width, _height);

        mapLayerMask = LayerMasks.GroundAndPlatform;
    }

    private void OnDrawGizmos()
    {
        var cc = gameObject.GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(cc.bounds.center, new Vector2(cc.size.x, cc.size.y));

        // IsStick 확인용
        Collider2D c = _player?.Collider;

        if (c == null) return;
        float width = c.bounds.size.x;
        float height = c.bounds.size.y;
        float castRadius = width / 2f;
        float castDist = height / 2f - castRadius;

        Vector2 castCenter = (Vector2)c.bounds.center + Vector2.down * castDist;

        // BoxCast 크기
        Vector2 boxSize = new Vector2(width, castRadius * 0.2f);
        Vector2 boxPosition = castCenter + castRadius * Vector2.down;

        // BoxCast를 Gizmos로 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxPosition, boxSize);
    }

    public void OnClimbAble()
    {
    }

    public void OffClimbAble()
    {
    }
    
    private bool isProcessing = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Player.IsDrop && ClimbAble())
        {
            if (!collision.gameObject.layer.Equals(Layers.Ground) && !collision.gameObject.layer.Equals(Layers.Platform))
                return;

            enteredColliders.Add(collision);
            isProcessing = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!Player.IsDrop && ClimbAble())
        {
            if (!Player.IsDrop && ClimbAble())
            {
                if (!collision.gameObject.layer.Equals(Layers.Ground) && !collision.gameObject.layer.Equals(Layers.Platform))
                    return;

                enteredColliders.Add(collision);
                isProcessing = true;
            }
        }
    }

    void Update()
    {
        if (!isProcessing) return;

        isProcessing = false;

        Collider2D platfrom = null;
        foreach (var collider in enteredColliders)
        {
            if (collider == null || !collider.enabled) continue;

            if (collider.gameObject.layer.Equals(Layers.Platform))
            {
                platfrom = collider;
                continue; // 플랫폼은 나중에 처리
            }

            if (Climb(collider))
            {
                enteredColliders.Clear();
                return;
            }
        }

        Climb(platfrom);

        enteredColliders.Clear();
    }

    private bool Climb(Collider2D collision)
    {
        if (collision == null || !collision.enabled) return false;
        
        Transform incoming = collision.transform;

        if (!Utils.CheckLayer(LayerMasks.GroundAndPlatform, incoming.gameObject)) return false;

        /* ledge climb 여부 확인 */
        if (!collision.gameObject.layer.Equals(Layers.Platform))
            if (ClimbLedge()) return true;

        /* 이동 플랫폼은 등반 x */
        if (collision.TryGetComponent(out MovingObj movingObj))
            return false;

        /* platform climb 여부 확인 */
        if (ClimbPlatform()) return true;

        /* climb 실행 안함 */
        return false;
    }
    private bool ClimbAble()
    {
        // 이미 등반 중이거나 등반 가능한 상태가 아니거나 정면에 등반할 벽/플랫폼이 없으면 false
        if (Player.IsClimb || !Player.GetAbleState(EPlayerState.Climb) ||
        !Player.ActorMovement.CheckClimb()) return false;

        // 등반은 공중 동작 시 최고점 이후부터 가능하도록(점프 상승 중 등반 막기 위해, 대쉬 중은 예외) 
        if (!Player.IsDash && Player.Rb.velocity.y >= 0) return false;

        return true;
    }

    /* 절벽 등반 함수 */
    private bool ClimbLedge()
    {
        Vector2 pos = col.bounds.center;
        int scale = (int)Player.Direction;

        /* ledge climb는 벽 방향으로 이동 키 누르는 중에만 가능 */
        if (_player.PressingDir != _player.Direction || !(_player.IsMove || _player.IsDash))
            return false;

        Vector2 rayOrigin = (Vector2)col.bounds.center + new Vector2(_width * 0.3f * scale, _height * 0.5f);

        /* ledge 위쪽으로 등반 공간 있는지 확인 */
        RaycastHit2D hit;
        hit = Physics2D.Raycast(new Vector2(pos.x, pos.y + _height * 0.5f + 0.1f), Vector2.right * scale, searchWidth, mapLayerMask);

        Debug.DrawRay(new Vector2(pos.x, pos.y + _height * 0.5f + 0.1f), scale * searchWidth * Vector2.right, Color.red);
        Debug.DrawRay(new Vector2(pos.x, pos.y + _height * 0.5f + 0.1f), _height * climbDepth * Vector2.down, Color.red);

        /* 벽 위에 등반할 공간 없으면 false */
        if (hit.normal.sqrMagnitude > 0)
            return false;

        /* climb collider 상단 중심부터 아래로 raycast => 도착 위치 확인 */
        hit = Physics2D.Raycast(rayOrigin, Vector2.down, _height * climbDepth, mapLayerMask);

        Debug.DrawRay(rayOrigin, _height * climbDepth * Vector2.down, Color.blue);

        /* 도착 위치가 존재하지 않거나 threashold 이상 경계면 등반 불가 */
        if (hit.normal.sqrMagnitude <= 0 || Vector2.Angle(hit.normal, Vector2.up) > _player.ActorMovement.SlopeAngleThreshold)
            return false;

        Vector2 edgePos = hit.point;
        climbEndPos = edgePos;

        _playRatio = (rayOrigin.y - edgePos.y) / _height;   // 등반 애니메이션 재생 비율(등반 해야하는 높이에 따라 결정)

        var _type = edgePos.y >= _player.Collider.bounds.center.y ? EClimbType.LEDGEHIGH : EClimbType.LEDGELOW;   // 등반 모션 종류(높은 등반, 낮은 등반)

        _player.AddInfo(EPlayerState.Climb, new ClimbInfo { endPos = climbEndPos, type = _type, hit = hit.collider });

        _player.SetState(EPlayerState.Climb);

        return true;
    }

    /* 플랫폼 등반 함수                                */
    /* 플랫폼에서는 x 방향 이동 없이 y방향으로만 등반 실행  */ 
    private bool ClimbPlatform()
    {
        var playerCol = _player.Collider;
        float height = playerCol.bounds.size.y;
        Vector2 rayOrigin = (Vector2)playerCol.bounds.center + new Vector2(0, height * 0.5f);

        /* 플레이어 머리 위에서 아래로 raycast => 도착 위치 확인 */
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, height * climbDepth, mapLayerMask);

        Debug.DrawRay(rayOrigin, height * climbDepth * Vector2.down, Color.blue);

        /* 도착 위치가 존재하지 않거나 threashold 이상 경계면 등반 불가 */
        if (hit.normal.sqrMagnitude <= 0 || Vector2.Angle(hit.normal, Vector2.up) > _player.ActorMovement.SlopeAngleThreshold)
            return false;

        Vector2 edgePos = hit.point;
        climbEndPos = edgePos;

        _playRatio = (rayOrigin.y - edgePos.y) / _height;   // 등반 애니메이션 재생 비율(등반 해야하는 높이에 따라 결정)

        var _type = edgePos.y >= _player.Collider.bounds.center.y ? EClimbType.PLATFORMHIGH : EClimbType.PLATFORMLOW;   // 등반 모션 종류(높은 등반, 낮은 등반)

        _player.AddInfo(EPlayerState.Climb, new ClimbInfo { endPos = climbEndPos, type = _type, hit = hit.collider });

        _player.SetState(EPlayerState.Climb);

        return true;
    }
}
