using System.Collections;
using UnityEngine;


public abstract class Subject : MonoBehaviour
{
    // 옵저버 패턴 서브젝트

    readonly ArrayList observers = new();

    public void Attach(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Detach(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (IObserver observer in observers)
        {
            observer.Notify(this);
        }
    }
}

