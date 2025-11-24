using System;
using System.Collections;
using System.Collections.Generic;

public class CustomQueue<T> : IEnumerable<T>
{
    private List<T> list = new();

    public int Count => list.Count;

    public T this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }
    public void Enqueue(T element)
    {
        list.Add(element);
    }

    public T Dequeue()
    {
        if (list.Count == 0)
        {
            throw new Exception("Queue Out Of Index");
        }

        T element = list[0];
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
    public IEnumerator<T> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int IndexOf(T t)
    {
        return list.IndexOf(t);
    }
}
