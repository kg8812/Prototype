using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine.Events;

public partial class Actor : IEventUser
{
    private BuffEvent _buffEvent;

    private CollisionEventHandler _collisionEventHandler;

    private List<IEventChild> _eventChildren;
    private BuffEvent buffEvent => _buffEvent ??= gameObject.GetOrAddComponent<BuffEvent>();

    private CollisionEventHandler collisionEventHandler =>
        _collisionEventHandler ??= gameObject.GetOrAddComponent<CollisionEventHandler>();

    public IEventManager EventManager => buffEvent;

    public List<IEventChild> EventChildren => _eventChildren ??= new List<IEventChild>
    {
        buffEvent, collisionEventHandler
    };

    public void AddEvent(EventType eventType, UnityAction<EventParameters> action)
    {
        EventManager?.AddEvent(eventType, action);
    }

    public void RemoveEvent(EventType eventType, UnityAction<EventParameters> action)
    {
        EventManager?.RemoveEvent(eventType, action);
    }

    public void ExecuteEvent(EventType eventType, EventParameters parameters)
    {
        EventManager?.ExecuteEvent(eventType, parameters);
    }
}