using System;
using System.Collections;
using System.Collections.Generic;

public class CustomQueue<T> : IEnumerable<T>
{
    private readonly List<T> list = new();

    public int Count => list.Count;

    public T this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }

    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enqueue(T element)
    {
        list.Add(element);
    }

    public T Dequeue()
    {
        if (list.Count == 0) throw new Exception("Queue Out Of Index");

        var element = list[0];
        list.RemoveAt(0);

        return element;
    }

    public void Remove(T element)
    {
        list.Remove(element);
    }

    public void Clear()
    {
        list.Clear();
    }

    public int IndexOf(T t)
    {
        return list.IndexOf(t);
    }
}