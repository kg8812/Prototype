using System;
using Apis;
using DG.Tweening;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IActorRenderer))]
public abstract partial class Actor : MonoBehaviour, IOnHit, IOnHitReaction, IAttackable, IDirection, IAnimator
{
    #region Inspector Varieties

    [Tooltip("플레이어 충돌 시 밀어냄 발생 유무")]
    [LabelText("플레이어 밀침")]
    public bool IsResist = true;

    [LabelText("최대 속도")]
    public float maxVelocity;

    public Transform effectParent;

    #endregion

    #region Varieties

    private Collider2D _collider;
    private EActorDirection _direction = EActorDirection.Right;
    private EffectSpawner _effectSpawner;
    private Transform _thisTrans;

    private Collider2D hitCollider;

    private ActorCombat _actorCombat;

    protected IActorRenderer actorRenderer;

    #endregion

    #region Properties

    public virtual Collider2D HitCollider => hitCollider;

    public Rigidbody2D Rb { get; protected set; }

    public IActorRenderer ActorRenderer => actorRenderer ??= GetComponent<IActorRenderer>();

    public Collider2D Collider
    {
        get
        {
            if (_collider == null) _collider = transform.Find("Collision")?.GetComponent<Collider2D>();
            return _collider;
        }
    }

    public bool onAir { get; set; }

    public bool ableAttack { get; protected set; }

    public virtual EffectSpawner EffectSpawner => _effectSpawner ??= new EffectSpawner(this);

    private ActorCombat ActorCombat => _actorCombat ??= new(this);

    public bool IsPause { get; set; } // 애니메이션 멈춤 여부 (코루틴 멈추는데 사용할 것)

    public virtual Animator animator { get; set; }

    public virtual Vector3 Position
    {
        get => actorRenderer.GetPosition();
        set => actorRenderer.SetPosition(value);
    }

    public EActorDirection Direction
    {
        get
        {
            ResetDirection();
            return _direction;
        }
        set => _direction = value;
    }

    public int DirectionScale => (int)Direction;

    public virtual bool IsAffectedByCC => true;

    public bool IsDead { get; set; }

    public virtual int Exp => 5;

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        actorRenderer = GetComponent<IActorRenderer>();

        IsDead = false;
        EventChildren.ForEach(x => x.Init(this));
        hitCollider = GetComponent<Collider2D>();
        ResetDirection();
    }

    protected virtual void Start()
    {
        ResetCurHp();
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
        if (maxVelocity > 0)
            Rb.linearVelocity = Vector2.ClampMagnitude(Rb.linearVelocity, maxVelocity);
    }

    protected virtual void OnDestroy()
    {
        EventManager.ExecuteEvent(EventType.OnDestroy, new EventParameters(this));
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
    }

    #endregion

    #region 추상 메서드

    public abstract void IdleOn();

    public abstract void AttackOn();

    public abstract void AttackOff();

    #endregion

    #region 애니메이션

    public void StopAnimation()
    {
        animator.speed = 0;
    }

    public void ResumeAnimation()
    {
        animator.speed = 1;
    }

    public virtual void AnimPauseOn()
    {
        animator.speed = 0;
        this.DOPause();
        IsPause = true;
    }

    public virtual void AnimPauseOff()
    {
        animator.speed = 1;
        this.DOPlay();
        IsPause = false;
    }

    #endregion

    #region 전투관련 (공격 및 피격)

    public EventParameters Attack(EventParameters eventParameters)
    {
        return ActorCombat.Attack(eventParameters);
    }

    public virtual float OnHit(EventParameters parameters)
    {
        var value = ActorCombat.ReceiveHit(parameters);

        if (!IsDead)
            OnHitReaction(parameters);

        return value;
    }

    public virtual KnockBackData GetKnockBackData(EventParameters parameters)
    {
        return parameters.Get<KnockBackData>();
    }

    protected virtual void OnHitReaction(EventParameters eventParameters)
    {
        // 피격시 효과
    }

    #endregion

    #region 체력,죽음

    protected void ResetCurHp()
    {
        SetHpWithoutEvent(MaxHp);
        ExecuteEvent(EventType.OnHpHeal, new EventParameters(this));
        IsDead = false;
        ResetTextVariables();
    }

    public virtual void Die()
    {
        SetHpWithoutEvent(0);

        ExecuteEvent(EventType.OnDeath, new EventParameters(this));

        if (this is Monster)
            GameManager.instance.Player?.ExecuteEvent(
                EventType.OnKill,
                new EventParameters(GameManager.instance.Player, this)
            );

        IsDead = true;
    }

    #endregion

    #region Direction

    public virtual void SetDirection(EActorDirection dir)
    {
        var size = transform.localScale;
        _direction = dir;
        transform.localScale = new Vector3((int)dir * Mathf.Abs(size.x), size.y, size.z);
    }

    public void ResetDirection()
    {
        _direction = transform.localScale.x > 0 ? EActorDirection.Right : EActorDirection.Left;
    }

    public virtual void Flip()
    {
        SetDirection((EActorDirection)((int)_direction * -1));
    }

    #endregion

    #region 유틸

    public void ReturnToPool()
    {
        GameManager.Factory.Return(gameObject);
    }

    public virtual void SetEffectParent(GameObject effect)
    {
        effect.transform.SetParent(effectParent);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localRotation = Quaternion.identity;
        effect.transform.localScale = Vector3.one;
        effect.transform.SetParent(transform);
    }

    #endregion
}