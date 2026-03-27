using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority> : IEnumerable<(TElement Element, TPriority Priority)>
    where TPriority : IComparable<TPriority>
{
    private readonly List<(TElement Element, TPriority Priority)> _heap = new();
    private readonly int _maxSize;

    public PriorityQueue(int maxSize = -1)
    {
        _maxSize = maxSize;
    }

    public int Count => _heap.Count;

    public IEnumerator<(TElement Element, TPriority Priority)> GetEnumerator()
    {
        return _heap.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Enqueue(TElement element, TPriority priority)
    {
        var index = _heap.FindIndex(x => x.Element.Equals(element));

        if (index != -1)
        {
            _heap[index] = (element, priority);

            HeapifyUp(index);
            HeapifyDown(index);
        }
        else
        {
            _heap.Add((element, priority));
            HeapifyUp(_heap.Count - 1);
        }

        if (_maxSize == -1) return true;

        if (_heap.Count > _maxSize)
            if (RemoveMax().Equals(element))
                return false;

        return true;
    }

    public TElement Dequeue()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        var frontItem = _heap[0].Element;
        _heap[0] = _heap[^1];
        _heap.RemoveAt(_heap.Count - 1);

        if (_heap.Count > 0)
            HeapifyDown(0);

        return frontItem;
    }

    public TElement Front()
    {
        if (_heap.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        return _heap[0].Element;
    }

    public bool Remove(TElement element)
    {
        var index = _heap.FindIndex(x => EqualityComparer<TElement>.Default.Equals(x.Element, element));

        if (index == -1)
            return false;

        // 마지막 원소를 삭제할 원소의 위치로 옮긴 후 삭제
        _heap[index] = _heap[^1];
        _heap.RemoveAt(_heap.Count - 1);

        // 힙 속성 복원 (HeapifyUp 또는 HeapifyDown)
        if (index < _heap.Count)
        {
            HeapifyDown(index);
            HeapifyUp(index);
        }

        return true;
    }

    private TElement RemoveMax()
    {
        var maxIndex = 0;
        for (var i = 1; i < _heap.Count; i++)
            if (_heap[i].Priority.CompareTo(_heap[maxIndex].Priority) > 0)
                maxIndex = i;

        var result = _heap[maxIndex].Element;
        _heap.RemoveAt(maxIndex);
        return result;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            var parentIndex = (index - 1) / 2;
            if (_heap[index].Priority.CompareTo(_heap[parentIndex].Priority) >= 0)
                break;

            Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    private void HeapifyDown(int index)
    {
        int leftChild, rightChild, smallest;
        while (true)
        {
            leftChild = 2 * index + 1;
            rightChild = 2 * index + 2;
            smallest = index;

            if (leftChild < _heap.Count && _heap[leftChild].Priority.CompareTo(_heap[smallest].Priority) < 0)
                smallest = leftChild;

            if (rightChild < _heap.Count && _heap[rightChild].Priority.CompareTo(_heap[smallest].Priority) < 0)
                smallest = rightChild;

            if (smallest == index)
                break;

            Swap(index, smallest);
            index = smallest;
        }
    }

    private void Swap(int i, int j)
    {
        var temp = _heap[i];
        _heap[i] = _heap[j];
        _heap[j] = temp;
    }

    public void Clear()
    {
        _heap.Clear();
    }
}