using System.Collections.Generic;
using UnityEngine;

public class ItemStorage
{
    private readonly GameObject parent;
    private readonly HashSet<Item> storage = new();

    public ItemStorage(string name)
    {
        parent = new GameObject(name);
        Object.DontDestroyOnLoad(parent);
    }

    public void Store(Item item)
    {
        if (storage.Add(item)) item.transform.SetParent(parent.transform);
    }

    public Item Get(Item item)
    {
        return Get<Item>(item);
    }

    public T Get<T>(T item) where T : Item
    {
        if (storage.Remove(item)) return item;
        return null;
    }

    public void HardReset()
    {
        foreach (var st in storage) st.Return();
        storage.Clear();
    }
}