using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using UnityEngine;
using UnityEngine.Events;

public class StateEvent : IEventManager
{
    public IDictionary<EventType, UnityEvent<EventParameters>> EventDict
    {
        get; private set;
    } = new Dictionary<EventType, UnityEvent<EventParameters>>();

    public StateEvent()
    {
        foreach (EventType type in Utils.EventTypes)
        {
            EventDict.TryAdd(type, new UnityEvent<EventParameters>());
        }
    }

    public void AddEvent(EventType type, UnityAction<EventParameters> action)
    {
        RemoveEvent(type, action);
        if (EventDict.TryGetValue(type, out UnityEvent<EventParameters> userEvent))
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
        if (EventDict.ContainsKey(type))
        {
            EventDict[type].Invoke(parameters);
        }
    }

    public void ExecuteEventOnce(EventType type, EventParameters parameters)
    {
        if (EventDict.ContainsKey(type))
        {
            EventDict[type].Invoke(parameters);
            EventDict[type].RemoveAllListeners();
        }
    }

    public UnityEvent<EventParameters> GetEvent(EventType type)
    {
        if (EventDict.TryGetValue(type, out UnityEvent<EventParameters> userEvent))
        {
            return userEvent;
        }
        return null;
    }

    public void RemoveEvent(EventType type, UnityAction<EventParameters> action)
    {
        if (EventDict.TryGetValue(type, out UnityEvent<EventParameters> userEvent))
        {
            userEvent.RemoveListener(action);
        }
    }

    public void RemoveAllEvents(EventType type)
    {
        if (EventDict.TryGetValue(type, out UnityEvent<EventParameters> userEvent))
        {
            userEvent.RemoveAllListeners();
        }
    }
}
