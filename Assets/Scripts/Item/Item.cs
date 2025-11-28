using System;
using Apis;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Item : SerializedMonoBehaviour
{
    // public abstract string FlavourText { get; } // 아이템 플레이버 텍스트 
    // public abstract string Description { get; } // 아이템 효과 설명


    protected Actor user;

    [HideInInspector] public Action<Item> OnEquipped;
    [HideInInspector] public Action<Item> OnUnEquipped;

    public abstract int ItemId { get; } // 아이템 id

    // public abstract string Name { get; } // 아이템 이름
    public Sprite Image { get; protected set; } // 아이템 이미지 

    public ItemSlot slot { get; set; }

    public bool IsEquip { get; set; }

    public void Collect()
    {
        OnCollect();
    }

    public void Remove()
    {
        OnRemove();
    }

    protected virtual void OnCollect()
    {
    }

    protected virtual void OnRemove()
    {
    }

    public void Equip(IMonoBehaviour user)
    {
        if (IsEquip) return;
        // Debug.Log($"item has equipped {Name}");
        IsEquip = true;
        OnEquip(user);
    }

    protected virtual void OnEquip(IMonoBehaviour user)
    {
        this.user = user as Actor;
        this.user?.AddEvent(EventType.OnUpdate, UpdateFunc);
        OnEquipped?.Invoke(this);
    }

    public void UnEquip()
    {
        if (!IsEquip) return;
        // Debug.Log($"item has unequipped {Name}");
        IsEquip = false;
        OnUnEquip();
    }

    protected virtual void OnUnEquip()
    {
        user?.RemoveEvent(EventType.OnUpdate, UpdateFunc);
        OnUnEquipped?.Invoke(this);
    }

    public virtual void Init()
    {
        Activate();
    }

    public abstract void Activate();

    public virtual void Return()
    {
        UnEquip();
    }

    protected virtual void UpdateFunc(EventParameters _)
    {
    }

    /// <param name="trans">null이면 store로 보냄.</param>
    public virtual void SetParent(Transform trans)
    {
        if (ReferenceEquals(trans, null))
            InvenManager.instance.Storage.Store(this);
        else
            gameObject.transform.SetParent(trans);
    }
}