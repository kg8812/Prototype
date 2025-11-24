using System;
using System.Collections;
using System.Collections.Generic;

public class CustomStack<T> : IEnumerable<T>
{
    private List<T> list = new();

    public int Count => list.Count;

    public T this[int index]
    {
        get => list[index];
        set => list[index] = value;
    }
    public void Push(T element)
    {
        list.Add(element);
    }

    public T Pop()
    {
        if (list.Count == 0)
        {
            throw new Exception("Stack Out Of Index");
        }

        T element = list[^1];
        list.RemoveAt(list.Count - 1);

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