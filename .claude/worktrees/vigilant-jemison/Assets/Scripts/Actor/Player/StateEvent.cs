using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine.Events;

public class StateEvent : IEventManager
{
    public StateEvent()
    {
        foreach (var type in Utils.EventTypes) EventDict.TryAdd(type, new UnityEvent<EventParameters>());
    }

    public IDictionary<EventType, UnityEvent<EventParameters>> EventDict { get; } =
        new Dictionary<EventType, UnityEvent<EventParameters>>();

    public void AddEvent(EventType type, UnityAction<EventParameters> action)
    {
        RemoveEvent(type, action);
        if (EventDict.TryGetValue(type, out var userEvent))
        {
            userEvent.AddListener(action);
        }
        else
        {
            userEvent = new UnityEvent<EventParameters>();
            EventDict.Add(type, userEvent);
            userEvent.AddListener(action);
        }
    }

    public void ExecuteEvent(EventType type, EventParameters parameters)
    {
        if (EventDict.ContainsKey(type)) EventDict[type].Invoke(parameters);
    }

    public UnityEvent<EventParameters> GetEvent(EventType type)
    {
        if (EventDict.TryGetValue(type, out var userEvent)) return userEvent;
        return null;
    }

    public void RemoveEvent(EventType type, UnityAction<EventParameters> action)
    {
        if (EventDict.TryGetValue(type, out var userEvent)) userEvent.RemoveListener(action);
    }

    public void ExecuteEventOnce(EventType type, EventParameters parameters)
    {
        if (EventDict.ContainsKey(type))
        {
            EventDict[type].Invoke(parameters);
            EventDict[type].RemoveAllListeners();
        }
    }

    public void RemoveAllEvents(EventType type)
    {
        if (EventDict.TryGetValue(type, out var userEvent)) userEvent.RemoveAllListeners();
    }
}