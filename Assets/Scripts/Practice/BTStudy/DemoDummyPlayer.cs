using UnityEngine;

/// <summary>
///     BT 데모 테스트용 더미 플레이어. 방향키 이동 + 스페이스바 점프만 한다.
///     실제 Player는 GameManager/GameState가 매 프레임 ActorController.KeyControl()을 호출해줘야
///     움직이므로, GameManager가 없는 테스트 씬에서는 이 스크립트로 대체한다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class DemoDummyPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpPower = 9f;

    [Header("바닥 판정")]
    [SerializeField] private LayerMask groundMask = 1 << 6; // Ground

    [SerializeField] private float groundCheckDistance = 0.08f;

    private Collider2D _col;
    private bool _jumpQueued;
    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        // 입력은 Update에서 받고 실제 반영은 FixedUpdate에서 한다 (프레임 사이 입력 유실 방지)
        if (Input.GetKeyDown(KeyCode.Space)) _jumpQueued = true;
    }

    private void FixedUpdate()
    {
        var input = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) input -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) input += 1f;

        var velocity = _rb.linearVelocity;
        velocity.x = input * moveSpeed;

        if (_jumpQueued)
        {
            _jumpQueued = false;
            if (IsGrounded()) velocity.y = jumpPower;
        }

        _rb.linearVelocity = velocity;
    }

    private void OnDrawGizmosSelected()
    {
        var col = _col != null ? _col : GetComponent<Collider2D>();
        if (col == null) return;

        var bounds = col.bounds;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(
            new Vector3(bounds.center.x, bounds.min.y - groundCheckDistance * 0.5f, bounds.center.z),
            new Vector3(bounds.size.x * 0.9f, groundCheckDistance, 0f));
    }

    private bool IsGrounded()
    {
        if (_col == null) return true;

        var bounds = _col.bounds;

        // 발밑을 얇은 박스로 훑는다. 콜라이더 폭보다 살짝 좁게 잡아 벽에 붙었을 때 오판을 줄인다
        return Physics2D.BoxCast(bounds.center, new Vector2(bounds.size.x * 0.9f, 0.02f), 0f,
            Vector2.down, bounds.extents.y + groundCheckDistance, groundMask);
    }
}
