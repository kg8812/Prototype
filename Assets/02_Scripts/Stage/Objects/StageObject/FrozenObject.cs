using System;
using Default;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

public class FrozenObject : DestroyableObject ,IOnInteract
{
    [SerializeField] private Renderer _render;
     MaterialPropertyBlock _propBlock;
    MaterialPropertyBlock propBlock => _propBlock ??= new();
    
    public static readonly UnityEvent<GameObject> InteractEvent = new();
    public static bool isEnabled;
    
    private bool isDestroyed;

    public static Func<bool> InteractionCheck;

    public Func<bool> InteractCheckEvent { get; set; }

    private AttackTypeCheck _attackTypeCheck;
    protected override IDestroyCheck DestroyCheck => _attackTypeCheck ??= new(Define.AttackType.FrozenDrillAttack);

    bool Check()
    {
        return (InteractionCheck?.Invoke() ?? true) && !isDestroyed && isEnabled;
    }
    
    protected override void Awake()
    {
        base.Awake();
        isDestroyed = false;
        isEnabled = false;
        _render.GetPropertyBlock(propBlock);
        propBlock.SetInt("_IsIcy",1);
        _render.SetPropertyBlock(propBlock);
        InteractCheckEvent += Check;
        var interacts = transform.GetComponentsInParentAndChild<IOnInteract>();
        interacts.ForEach(x =>
        {
            if (x is not FrozenObject)
            {
                x.InteractCheckEvent += CheckDestroyed;
            }
        });
    }

    protected override void DestroyObj(EventParameters parameters)
    {
        base.DestroyObj(parameters);
        isDestroyed = true;
        _render.GetPropertyBlock(propBlock);
        propBlock.SetInt("_IsIcy",0);
        _render.SetPropertyBlock(propBlock);
    }

    bool CheckDestroyed()
    {
        return isDestroyed;
    }

    public void OnInteract()
    {
        isDestroyed = true;
        InteractEvent.Invoke(gameObject);
        DestroyObj(null);
    }
}
