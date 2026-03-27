using System;
using Apis;
using EventData;
using UnityEngine;

public class DestroyableObject : MonoBehaviour, IOnHit
{
    public bool hasBlock;
    private Collider2D blockCol;


    protected SpriteRenderer sr;
    private Collider2D triggerCol;

    public Vector3 TopPivot
    {
        get => transform.position;
        set => transform.position = value;
    }

    public float CritHit => 0;


    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (hasBlock)
            blockCol = transform.GetChild(0).GetComponent<Collider2D>();
        triggerCol = GetComponent<Collider2D>();

        Init();
    }

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

    public int Exp => 0;

    public float OnHit(EventParameters parameters)
    {
        if (IsDead) return 0;

        DestroyObj(parameters);


        return parameters.Get<AttackEventData>().dmg;
    }

    public float MaxHp => 10;

    public float CurHp { get; set; }

    public Vector3 Position
    {
        get => transform.position;
        set => transform.position = value;
    }

    public bool IsDead { get; private set; }

    public bool HitImmune { get; set; }

    public virtual void Init()
    {
        IsDead = false;
        triggerCol.enabled = true;
        if (hasBlock)
            blockCol.enabled = true;
    }

    protected virtual void DestroyObj(EventParameters parameters)
    {
        IsDead = true;
        triggerCol.enabled = false;
        if (hasBlock)
            blockCol.enabled = false;
    }
}