using System;
using Apis;
using Apis;
using Default;
using DG.Tweening;
using EventData;
using Managers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody2D))]
public abstract partial class Actor : MonoBehaviour, IOnHit, IOnHitReaction, IAttackable , IDirection, IAnimator
{
    private Collider2D hitCollider;
    public virtual Collider2D HitCollider => hitCollider;

    public Rigidbody2D Rb { get; protected set; }
    EActorDirection _direction = EActorDirection.Right;
    public virtual Animator animator { get; set; }

    Transform _thisTrans;
    public Transform thisTrans => _thisTrans ??= transform;
   
    protected MaterialPropertyBlock _propBlock;
    protected MaterialPropertyBlock propBlock => _propBlock ??= new();
    
    protected IActorRenderer actorRenderer;
    public IActorRenderer ActorRenderer => actorRenderer ??= GetComponent<IActorRenderer>();
    
    public Collider2D Collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = transform.Find("Collision")?.GetComponent<Collider2D>();
            }

            return _collider;
        }
    }

    private Collider2D _collider;

    public bool onAir { get; set; }
    public bool ableAttack { get; protected set; }

    [Tooltip("플레이어 충돌 시 밀어냄 발생 유무")]
    [LabelText("플레이어 밀침")]
    public bool IsResist = true;
    public virtual bool IsAffectedByCC => true;

    private ImmunityController _immunityController;
    public ImmunityController ImmunityController => _immunityController ??= new();
    private EffectSpawner _effectSpawner;
    public virtual EffectSpawner EffectSpawner => _effectSpawner ??= new(this);
    public abstract void IdleOn();
    public abstract void AttackOn();
    public abstract void AttackOff();

    public bool IsPause { get; set; } // 애니메이션 멈춤 여부 (코루틴 멈추는데 사용할 것)

    private Guid recentHitInfo; // 최근에 피격당한 공격 혹은 스킬 (중복 체크용)
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
    
    int layer;
    
    [LabelText("최대 속도")] public float maxVelocity;


    public virtual Vector3 Position
    {
        get => actorRenderer.GetPosition();

        set => actorRenderer.SetPosition(value);
    }

    public virtual float OnHit(EventParameters parameters)
    {
        if (IsInvincible || parameters == null || IsDead) return 0;
        
        ExecuteEvent(EventType.OnBeforeHit,parameters);

        if (parameters.hitData.hitDisable) return 0;
        
        BonusStatEvent += Action;

        if (parameters.hitData.dmg == 0)
        {
            parameters.hitData.dmg = parameters.atkData.dmg;
        }

        parameters.hitData.dmg *= 1 - (1 - FormulaConfig.defConstant / (FormulaConfig.defConstant + Def));

        parameters.hitData.dmg = Mathf.RoundToInt(parameters.hitData.dmg);

        ExecuteEvent(EventType.OnHit, parameters);
        if (parameters.hitData.isCritApplied)
        {
            ExecuteEvent(EventType.OnCritHit,parameters);
        }
        CurHp -= parameters.hitData.dmg;

        ExecuteEvent(EventType.OnAfterHit, parameters);
        BonusStatEvent -= Action;

        if(!IsDead)
            OnHitReaction(parameters);

        recentHitInfo = parameters.atkData.attackGuid;
        return parameters.hitData.dmg;

        BonusStat Action()
        {
            return parameters.statData.stat;
        }
    }

    protected virtual void OnHitReaction(EventParameters eventParameters)
    {
        // 피격시 효과
    }

    public virtual KnockBackData GetKnockBackData(EventParameters parameters)
    {
        return parameters.knockBackData;
    }

    private bool _isDead;

    public bool IsDead
    {
        get => _isDead;
        set => _isDead = value;
    }

    protected virtual void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();

        layer = gameObject.layer;
        actorRenderer = GetComponent<IActorRenderer>();
        
        IsDead = false;
        EventChildren.ForEach(x => x.Init(this));
        _immunityController = new();
        hitCollider = GetComponent<Collider2D>();
        ResetDirection();
    }

    public Guid AddInvincibility()
    {
        if (!ImmunityController.Contains("Invincible"))
        {
            ImmunityController.MakeNewType("Invincible");
        }

        return ImmunityController.AddCount("Invincible");
    }
    public void RemoveInvincibility(Guid guid)
    {
        ImmunityController.MinusCount("Invincible",guid);
    }

    public virtual int Exp => 5;

    public void ForceRemoveInvincibility()
    {
        gameObject.layer = layer;
        ImmunityController.MakeCountToZero("Invincible");
    }
    protected virtual void Start()
    {
        ResetCurHp();
        BonusStatEvent += () => SubBuffManager.Stats;
    }

    protected void ResetCurHp()
    {
        curHp = MaxHp;
        ExecuteEvent(EventType.OnHpHeal, new EventParameters(this));
        IsDead = false;
        ResetTextVariables();
    }

    protected virtual void Update()
    {
        SubBuffManager.Update();
    }
    protected virtual void FixedUpdate()
    {
        if (maxVelocity > 0)
        {
            Rb.linearVelocity = Vector2.ClampMagnitude(Rb.linearVelocity, maxVelocity);
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
    }
    public virtual void OnTriggerExit2D(Collider2D other)
    {
    }

    public virtual void Die()
    {
        curHp = 0;

        ExecuteEvent(EventType.OnDeath, new EventParameters(this));

        if (this is Monster)
        {
            GameManager.instance.Player?.ExecuteEvent(EventType.OnKill, new(GameManager.instance.Player, this));
        }

        IsDead = true;
        RemoveAllBuff();
    }
    
    public EventParameters Attack(EventParameters eventParameters)
    {
        if (IsDead || gameObject == null) return eventParameters;
        
        if (eventParameters?.target == null || eventParameters.target.IsInvincible)
        {
            return eventParameters;
        }    
        
        BonusStat Action()
        {
            return eventParameters.statData.stat;
        }
        BonusStatEvent += Action;
        
        // 이벤트 실행을 데미지 계산 전에 호출해야함
        // 데미지 증가, 크리티컬 확률 증가 등 효과들이 적용되어야 하기 때문
        if (eventParameters.target is Actor)
        {
            // 타격 성공 판정도 Actor 한정으로만 (다른 IOnHit는 불가)
            ExecuteEvent(EventType.OnAttackSuccess, eventParameters);
            if (eventParameters.atkData.attackType == Define.AttackType.BasicAttack)
            {
                ExecuteEvent(EventType.OnBasicAttack, eventParameters);
            }
        }
        
        eventParameters.atkData.dmg = eventParameters.atkData.atkStrategy.Calculate(eventParameters.target);
        float random = Random.Range(0, 100f);
        float prob = CritProb;
       
        if (random < prob || eventParameters.atkData.isfixedCrit)
        {
            eventParameters.atkData.dmg *= CritDmg * 0.01f;
            
            ExecuteEvent(EventType.OnCrit, eventParameters);
            eventParameters.hitData.isCritApplied = true;
        }
        else
        {
            eventParameters.hitData.isCritApplied = false;
        }
        
        if (eventParameters.target is Actor act)
        {
            if (Vector2.Dot(act.transform.right * (int)act._direction,
                    (transform.position - eventParameters.target.gameObject.transform.position).normalized) < 0)
            {
                ExecuteEvent(EventType.OnBackAttack, eventParameters);
            }
        }

        eventParameters.hitData.dmg = eventParameters.atkData.dmg;
        
        eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
        
        ExecuteEvent(EventType.OnAfterAtk, eventParameters);
        BonusStatEvent -= Action;
        
        return eventParameters;
    }

    public Transform effectParent;

    public void ResetDirection()
    {
        
        _direction = transform.localScale.x > 0 ? EActorDirection.Right : EActorDirection.Left;
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
        Vector3 size = transform.localScale;
        _direction = dir;
        transform.localScale = new Vector3((int)dir * Mathf.Abs(size.x), size.y, size.z);
    }
    
    public virtual void Flip()
    {
        SetDirection((EActorDirection)((int)_direction * -1));
    }

    //애니메이션에서 길이 조절용으로 사용합니다. 
    public void DummyFunction()
    {
        
    }

    public void StopAnimation()
    {
        animator.speed = 0;
    }

    public void ResumeAnimation()
    {
        animator.speed = 1;
    }
    public void ReturnToPool()
    {
        GameManager.Factory.Return(gameObject);
    }

    protected virtual void OnEnable()
    {
        
    }
    protected virtual void OnDestroy()
    {
        EventManager.ExecuteEvent(EventType.OnDestroy,new EventParameters(this));
    }

    public virtual void SetEffectParent(GameObject effect)
    {
        effect.transform.SetParent(effectParent);
        effect.transform.localPosition = Vector3.zero;
        effect.transform.localRotation = Quaternion.identity;
        effect.transform.localScale = Vector3.one;
    }
}
