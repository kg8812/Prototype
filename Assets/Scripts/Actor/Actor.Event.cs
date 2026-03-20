using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine.Events;

public partial class Actor : IEventUser
{
    private ActorEvents _actorEvents;

    public IEventManager EventManager => _actorEvents.EventManager;

    public List<IEventChild> EventChildren => _actorEvents.EventChildren;
    
    public void AddEvent(EventType eventType, UnityAction<EventParameters> action)
    {
        _actorEvents?.AddEvent(eventType, action);
    }

    public void RemoveEvent(EventType eventType, UnityAction<EventParameters> action)
    {
        _actorEvents?.RemoveEvent(eventType, action);
    }

    public void ExecuteEvent(EventType eventType, EventParameters parameters)
    {
        _actorEvents?.ExecuteEvent(eventType, parameters);
    }
}