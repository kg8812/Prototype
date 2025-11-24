using System;
using System.Collections.Generic;
using Apis;
using Apis;
using EventData;
using UnityEngine;

public class DestroyableObject : MonoBehaviour, IOnHit
{
    public float groggyRequired;
    public bool hasBlock;
    

    protected SpriteRenderer sr;
    private Collider2D blockCol;
    private Collider2D triggerCol;

    public bool IsAffectedByCC => false;
    public bool IsInvincible => false;

    private GroggyDestroyCheck _groggyDestroyCheck;
    protected virtual IDestroyCheck DestroyCheck => _groggyDestroyCheck ??= new(groggyRequired);
    
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

    public int Exp => 0;


    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (hasBlock)
            blockCol = transform.GetChild(0).GetComponent<Collider2D>();
        triggerCol = GetComponent<Collider2D>();
        
        Init();
    }

    public virtual void Init()
    {
        isDead = false;
        triggerCol.enabled = true;
        if (hasBlock)
            blockCol.enabled = true;
    }
    public float OnHit(EventParameters parameters)
    {
        if (isDead) return 0;
        if(DestroyCheck.CheckDestroyable(parameters))
        {
            DestroyObj(parameters);
        }

        return parameters.atkData.dmg;
    }
    
    protected virtual void DestroyObj(EventParameters parameters)
    {
        isDead = true;
        triggerCol.enabled = false;
        if(hasBlock)
            blockCol.enabled = false;
    }

    public float MaxHp => 10;

    public float CurHp { get; set; }

    private bool isDead;

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public Vector3 TopPivot
    {
        get => transform.position;
        set => transform.position = value;
    }

    public float CritHit => 0;

    public bool IsDead => isDead;

    public bool HitImmune { get; set; }
}
