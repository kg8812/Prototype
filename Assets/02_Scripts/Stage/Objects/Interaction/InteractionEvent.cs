using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class InteractionEvent : SerializedMonoBehaviour, IOnInteract
{
    public UnityEvent OnInteractEvent;

    public Func<bool> CheckEvent;

    private void Awake()
    {
        InteractCheckEvent += Check;
    }

    public Func<bool> InteractCheckEvent { get; set; }

    public void OnInteract()
    {
        OnInteractEvent?.Invoke();
    }

    private bool Check()
    {
        return CheckEvent == null || CheckEvent();
    }
}