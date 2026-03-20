using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine.Events;

public partial class Actor : IEventUser
{
    private ActorEvents _actorEvents;
    private ActorEvents actorEvents => _actorEvents ??= new(gameObject);
    public IEventManager EventManager => actorEvents.EventManager;

    public List<IEventChild> EventChildren => actorEvents.EventChildren;
    
    public void AddEvent(EventType eventType, UnityAction<EventParameters> action)
    {
        actorEvents?.AddEvent(eventType, action);
    }

    public void RemoveEvent(EventType eventType, UnityAction<EventParameters> action)
    {
        actorEvents?.RemoveEvent(eventType, action);
    }

    public void ExecuteEvent(EventType eventType, EventParameters parameters)
    {
        actorEvents?.ExecuteEvent(eventType, parameters);
    }
}