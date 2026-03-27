using System.Collections.Generic;
using Default;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class BuffEvent : MonoBehaviour, IEventManager, IEventChild
    {
        private EventParameters _parameters;
        private IEventUser _user;

        public IDictionary<EventType, UnityEvent<EventParameters>> EventDict { get; } =
            new Dictionary<EventType, UnityEvent<EventParameters>>();

        private void Awake()
        {
            foreach (var type in Utils.EventTypes) EventDict.TryAdd(type, new UnityEvent<EventParameters>());
        }

        private void Update()
        {
            ExecuteEvent(EventType.OnUpdate, _parameters);
        }

        private void FixedUpdate()
        {
            ExecuteEvent(EventType.OnFixedUpdate, _parameters);
        }

        private void OnEnable()
        {
            ExecuteEvent(EventType.OnEnable, new EventParameters(_user));
        }

        private void OnDisable()
        {
            if (!GameManager.IsQuitting)
                ExecuteEvent(EventType.OnDisable, new EventParameters(_user));
        }

        public void Init(IEventUser user)
        {
            _user = user;
            _parameters = new EventParameters(_user);
        }

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

        public UnityEvent<EventParameters> GetEvent(EventType type)
        {
            if (EventDict.TryGetValue(type, out var userEvent)) return userEvent;
            return null;
        }

        public void RemoveEvent(EventType type, UnityAction<EventParameters> action)
        {
            if (EventDict.TryGetValue(type, out var userEvent)) userEvent.RemoveListener(action);
        }

        public void ExecuteEvent(EventType type, EventParameters parameters)
        {
            if (EventDict.ContainsKey(type)) EventDict[type].Invoke(parameters);
        }
    }
}