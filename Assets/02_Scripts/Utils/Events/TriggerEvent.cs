using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour
{
    public UnityEvent<Collider2D> triggerEnterEvent;
    public UnityEvent<Collider2D> triggerExitEvent;

    private void OnTriggerEnter2D(Collider2D other)
    {
        triggerEnterEvent ??= new UnityEvent<Collider2D>();
        triggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        triggerExitEvent ??= new UnityEvent<Collider2D>();
        triggerExitEvent?.Invoke(other);
    }
}