using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis;
using Default;
using UnityEngine;

public class LaserTrap : MonoBehaviour,IOnHit,IAttackable
{
    public float maxHp;
    private float curHp;
    public LayerMask targetLayers;
    public int fireCount;
    public float laserWidth;
    public float laserDistance;
    public float chargeTime;
    public float duration;
    public ProjectileInfo laserInfo;
    public float cd;
    public float dmg;
    public LayerMask blockLayers;

    private bool isDead;

    private float curCd;
    private bool isFiring;
    private SpriteRenderer _renderer;

    private IMonoBehaviour _target;
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        CurHp = maxHp;
        isDead = false;
        curCd = 0;
        isFiring = false;
    }

    void Init()
    {
        isDead = false;
        curCd = 0;
        isFiring = false;
    }
    private void OnEnable()
    {
        Init();
    }

    IEnumerator StartFiring(IMonoBehaviour target)
    {
        if (isFiring) yield break;
        curCd = cd;
        isFiring = true;

        for (int i = 0; i < fireCount; i++)
        {
            _renderer.color = Color.yellow;
            yield return new WaitForSeconds(chargeTime);
            FireLaser(target);
            _renderer.color = Color.red;
        }
        isFiring = false;
    }
    
    void FireLaser(IMonoBehaviour target)
    {
        float distance = laserDistance;
        Vector2 dir = target.Position - transform.position;
        var ray = Physics2D.Raycast(transform.position, dir, distance, blockLayers);
        if (ray.collider != null)
        {
            distance = Mathf.Clamp(distance, 0, ray.distance);
        }
        
        dir.Normalize();
        Vector2 midPos = (Vector2)transform.position + dir * distance / 2; 

        AttackObject laser = GameManager.Factory.Get<AttackObject>(FactoryManager.FactoryType.AttackObject,
            Define.StageObjects.Laser, midPos);
        laser.Init(this,new AtkBase(this,laserInfo.dmg),duration);
        laser.Init(laserInfo);
        laser.transform.right = -dir;
        laser.transform.localScale = new Vector2(distance, laserWidth);

    }

    private void Update()
    {
        if (curCd > 0 && !isFiring)
        {
            curCd -= Time.deltaTime;
        }

        if (curCd <= 0 && _target != null)
        {
            StartCoroutine(StartFiring(_target));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (Utils.CheckLayer(targetLayers, other.gameObject.layer))
        {
            _target = other.gameObject.GetComponent<IMonoBehaviour>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (Utils.CheckLayer(targetLayers, other.gameObject.layer))
        {
            _target = null;
        }
    }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector3 TopPivot
    {
        get => Vector2.zero;
        set
        {
        }
    }

    public float OnHit(EventParameters parameters)
    {
        if (isDead) return 0;

        CurHp -= parameters.atkData.dmg;

        return parameters.atkData.dmg;
    }

    private TextShow _dmgText;
    TextShow DmgText => _dmgText ??= new DmgTextShow();


    public float MaxHp => maxHp;

    public float CurHp
    {
        get => curHp;
        set
        {
            int hitDmg = Mathf.RoundToInt(curHp - value);
            
            curHp = value;
            if (!Mathf.Approximately(hitDmg, 0) && hitDmg > 0)
            {
                DmgText?.Show(hitDmg,Position);
            }
            if (curHp <= 0)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        curHp = 0;
        gameObject.SetActive(false);
        isDead = true;
    }

    public float CritHit => 0;

    public bool IsDead => isDead;

    public bool HitImmune => true;

    public bool IsAffectedByCC => false;
    
    public bool IsInvincible => false;

    public Guid AddInvincibility()
    {
        return Guid.Empty;
    }

    public void RemoveInvincibility(Guid guid)
    {
    }

    public Guid AddHitImmunity()
    {
        return Guid.Empty;
    }

    public void RemoveHitImmunity(Guid guid)
    {
    }

    public int Exp => 5;

    public float Atk => dmg;

    public void AttackOn()
    {
    }

    public void AttackOff()
    {
    }

    public EventParameters Attack(EventParameters eventParameters)
    {
        if (eventParameters?.target == null || eventParameters.target.IsInvincible)
        {
            return null;
        }
        eventParameters.atkData.dmg = eventParameters.atkData.atkStrategy.Calculate(eventParameters.target);
            
        eventParameters.hitData.isCritApplied = false;

        eventParameters.hitData.dmg = eventParameters.atkData.dmg;

        eventParameters.hitData.dmgReceived = eventParameters.target.OnHit(eventParameters);
        return eventParameters;
    }
}
