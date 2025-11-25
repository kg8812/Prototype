using System.Collections.Generic;
using Apis;
using UnityEngine;

public class ItemStorage
{
    private HashSet<Item> storage = new();
    private readonly GameObject parent;

    public ItemStorage(string name)
    {
        parent = new GameObject(name);
        Object.DontDestroyOnLoad(parent);
    }
    public void Store(Item item)
    {
        if (storage.Add(item))
        {
            item.transform.SetParent(parent.transform);
        }
    }

    public Item Get(Item item)
    {
        return Get<Item>(item);
    }
    
    public T Get<T>(T item) where T : Item
    {
        if (storage.Remove(item))
        {
            return item;
        }
        return null;
    }

    public void HardReset()
    {
        foreach (var st in storage)
        {
            st.Return();
        }
        storage.Clear();
    }
}
