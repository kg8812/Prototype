using Apis;
using Default;
using UnityEngine;
using UnityEngine.Diagnostics;

public class BTMonster : Actor , IMovable
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    [Header("공격 연출 (데모용)")]
    [SerializeField] [Tooltip("비워두면 자식에서 자동으로 찾는다")]
    private Renderer visual;

    [SerializeField] private Color attackColor = new(1f, 0.85f, 0.2f);

    [SerializeField] [Tooltip("AttackOff가 안 불려도 이 시간이 지나면 원래 색으로 복귀")]
    private float attackFlashTime = 0.15f;

    private MaterialPropertyBlock _block;
    private Color _idleColor = Color.white;
    private Color _appliedColor;
    private bool _hasApplied;
    private bool _isFlashing;
    private float _flashEndTime;

    protected override void Awake()
    {
        base.Awake();

        if (visual == null) visual = GetComponentInChildren<Renderer>();
        if (visual == null) return;

        _block = new MaterialPropertyBlock();

        // URP Lit은 _BaseColor, Built-in Standard는 _Color를 쓴다. 있는 쪽에서 원래 색을 읽는다
        var mat = visual.sharedMaterial;
        if (mat == null) return;

        if (mat.HasProperty(BaseColorId)) _idleColor = mat.GetColor(BaseColorId);
        else if (mat.HasProperty(ColorId)) _idleColor = mat.GetColor(ColorId);
    }

    protected override void Update()
    {
        base.Update();

        if (_isFlashing && Time.time >= _flashEndTime) AttackOff();
    }

    public override void IdleOn()
    {
        if (_isFlashing) return;

        ApplyColor(_idleColor);
    }

    public override void AttackOn()
    {
        _flashEndTime = Time.time + attackFlashTime;
        _isFlashing = true;
        ApplyColor(attackColor);
    }

    public override void AttackOff()
    {
        _isFlashing = false;
        ApplyColor(_idleColor);
    }

    /// <summary>
    ///     sharedMaterial을 직접 바꾸면 머티리얼 에셋 자체가 수정되므로 PropertyBlock으로만 덮어쓴다.
    /// </summary>
    private void ApplyColor(Color color)
    {
        if (visual == null || _block == null) return;
        if (_hasApplied && _appliedColor == color) return;

        visual.GetPropertyBlock(_block);
        _block.SetColor(BaseColorId, color);
        _block.SetColor(ColorId, color);
        visual.SetPropertyBlock(_block);

        _appliedColor = color;
        _hasApplied = true;
    }

    UnitMoveComponent _moveComponent;
    public UnitMoveComponent MoveComponent
    {
        get
        {
            if (_moveComponent == null)
            {
                _moveComponent ??= gameObject.GetOrAddComponent<UnitMoveComponent>();
                _moveComponent.Init(this,Collider);
            }
            return _moveComponent;
        }
    }
    
    
}
