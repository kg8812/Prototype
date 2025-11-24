using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class InteractionEvent : SerializedMonoBehaviour,IOnInteract
{
    public UnityEvent OnInteractEvent;

    public Func<bool> CheckEvent;

    bool Check()
    {
        return CheckEvent == null || CheckEvent();
    }
    private void Awake()
    {
        InteractCheckEvent += Check;
    }

    public Func<bool> InteractCheckEvent { get; set; }

    public void OnInteract()
    {
        OnInteractEvent?.Invoke();
    }
}
