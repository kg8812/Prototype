using System;
using Apis;
using DG.Tweening;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IActorRenderer))]
public abstract partial class Actor : MonoBehaviour, IOnHit, IOnHitReaction, IAttackable, IDirection, IAnimator , IImmunity
{
    [Tooltip("플레이어 충돌 시 밀어냄 발생 유무")] [LabelText("플레이어 밀침")]
    public bool IsResist = true;

    [LabelText("최대 속도")] public float maxVelocity;

    public Transform effectParent;

    private Collider2D _collider;
    private EActorDirection _direction = EActorDirection.Right;
    private EffectSpawner _effectSpawner;

    private ImmunityController _immunityController;

    private Transform _thisTrans;

    protected IActorRenderer actorRenderer;
    private Collider2D hitCollider;

    private int layer;

    private Guid recentHitInfo; // 최근에 피격당한 공격 혹은 스킬 (중복 체크용)
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

    public bool IsPause { get; set; } // 애니메이션 멈춤 여부 (코루틴 멈추는데 사용할 것)

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();

        layer = gameObject.layer;
        actorRenderer = GetComponent<IActorRenderer>();

        IsDead = false;
        EventChildren.ForEach(x => x.Init(this));
        _immunityController = new ImmunityController();
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
        if (maxVelocity > 0) Rb.linearVelocity = Vector2.ClampMagnitude(Rb.linearVelocity, maxVelocity);
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

    public virtual Animator animator { get; set; }
    public abstract void IdleOn();

    public void StopAnimation()
    {
        animator.speed = 0;
    }

    public void ResumeAnimation()
    {
        animator.speed = 1;
    }

    public abstract void AttackOn();
    public abstract void AttackOff();

    public EventParameters Attack(EventParameters eventParameters)
    {
        if (IsDead || gameObject == null) return eventParameters;

        if (eventParameters?.target == null || eventParameters.target.IsInvincible) return eventParameters;

        BonusStat Action()
        {
            return eventParameters.Get<StatEventData>().stat;
        }

        BonusStatEvent += Action;

        // 이벤트 실행을 데미지 계산 전에 호출해야함
        // 데미지 증가, 크리티컬 확률 증가 등 효과들이 적용되어야 하기 때문
        if (eventParameters.target is Actor)
        {
            // 타격 성공 판정도 Actor 한정으로만 (다른 IOnHit는 불가)
            ExecuteEvent(EventType.OnAttackSuccess, eventParameters);
            if (eventParameters.Get<AttackEventData>().attackType == Define.AttackType.BasicAttack)
                ExecuteEvent(EventType.OnBasicAttack, eventParameters);
        }

        eventParameters.Get<AttackEventData>().dmg = eventParameters.Get<AttackEventData>().atkStrategy.Calculate(eventParameters.target);
        var random = Random.Range(0, 100f);
        var prob = CritProb;

        if (random < prob || eventParameters.Get<AttackEventData>().isfixedCrit)
        {
            eventParameters.Get<AttackEventData>().dmg *= CritDmg * 0.01f;

            ExecuteEvent(EventType.OnCrit, eventParameters);
            eventParameters.Get<HitEventData>().isCritApplied = true;
        }
        else
        {
            eventParameters.Get<HitEventData>().isCritApplied = false;
        }

        if (eventParameters.target is Actor act)
            if (Vector2.Dot(act.transform.right * (int)act._direction,
                    (transform.position - eventParameters.target.gameObject.transform.position).normalized) < 0)
                ExecuteEvent(EventType.OnBackAttack, eventParameters);

        eventParameters.Get<HitEventData>().dmg = eventParameters.Get<AttackEventData>().dmg;

        eventParameters.Get<HitEventData>().dmgReceived = eventParameters.target.OnHit(eventParameters);

        ExecuteEvent(EventType.OnAfterAtk, eventParameters);
        BonusStatEvent -= Action;

        return eventParameters;
    }

    public ImmunityController ImmunityController => _immunityController ??= new ImmunityController();


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

    public virtual void SetDirection(EActorDirection dir)
    {
        var size = transform.localScale;
        _direction = dir;
        transform.localScale = new Vector3((int)dir * Mathf.Abs(size.x), size.y, size.z);
    }

    public virtual bool IsAffectedByCC => true;

    public virtual float OnHit(EventParameters parameters)
    {
        if (IsInvincible || parameters == null || IsDead) return 0;

        ExecuteEvent(EventType.OnBeforeHit, parameters);

        if (parameters.Get<HitEventData>().hitDisable) return 0;

        BonusStatEvent += Action;

        if (parameters.Get<HitEventData>().dmg == 0) parameters.Get<HitEventData>().dmg = parameters.Get<AttackEventData>().dmg;

        parameters.Get<HitEventData>().dmg *= 1 - (1 - FormulaConfig.defConstant / (FormulaConfig.defConstant + Def));

        parameters.Get<HitEventData>().dmg = Mathf.RoundToInt(parameters.Get<HitEventData>().dmg);

        ExecuteEvent(EventType.OnHit, parameters);
        if (parameters.Get<HitEventData>().isCritApplied) ExecuteEvent(EventType.OnCritHit, parameters);
        CurHp -= parameters.Get<HitEventData>().dmg;

        ExecuteEvent(EventType.OnAfterHit, parameters);
        BonusStatEvent -= Action;

        if (!IsDead)
            OnHitReaction(parameters);

        recentHitInfo = parameters.Get<AttackEventData>().attackGuid;
        return parameters.Get<HitEventData>().dmg;

        BonusStat Action()
        {
            return parameters.Get<StatEventData>().stat;
        }
    }

    public bool IsDead { get; set; }

    public Guid AddInvincibility()
    {
        if (!ImmunityController.Contains("Invincible")) ImmunityController.MakeNewType("Invincible");

        return ImmunityController.AddCount("Invincible");
    }

    public void RemoveInvincibility(Guid guid)
    {
        ImmunityController.MinusCount("Invincible", guid);
    }

    public virtual int Exp => 5;

    public virtual KnockBackData GetKnockBackData(EventParameters parameters)
    {
        return parameters.Get<KnockBackData>();
    }

    public bool CheckDuplicationAtk(AttackObject atkObj)
    {
        return recentHitInfo != Guid.Empty && atkObj.firedAtkGuid == recentHitInfo;
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

    protected virtual void OnHitReaction(EventParameters eventParameters)
    {
        // 피격시 효과
    }

    public void ForceRemoveInvincibility()
    {
        gameObject.layer = layer;
        ImmunityController.MakeCountToZero("Invincible");
    }

    protected void ResetCurHp()
    {
        curHp = MaxHp;
        ExecuteEvent(EventType.OnHpHeal, new EventParameters(this));
        IsDead = false;
        ResetTextVariables();
    }

    public virtual void Die()
    {
        curHp = 0;

        ExecuteEvent(EventType.OnDeath, new EventParameters(this));

        if (this is Monster)
            GameManager.instance.Player?.ExecuteEvent(EventType.OnKill,
                new EventParameters(GameManager.instance.Player, this));

        IsDead = true;
    }

    public void ResetDirection()
    {
        _direction = transform.localScale.x > 0 ? EActorDirection.Right : EActorDirection.Left;
    }

    public virtual void Flip()
    {
        SetDirection((EActorDirection)((int)_direction * -1));
    }

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
    }
}