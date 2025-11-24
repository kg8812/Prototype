using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent<Collider2D> triggerEnterEvent;
    public UnityEvent<Collider2D> triggerExitEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        triggerEnterEvent ??= new();
        triggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        triggerExitEvent ??= new();
        triggerExitEvent?.Invoke(other);
    }
}
