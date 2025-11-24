using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Apis.Components
{
    public class TriggerManager
    {
        private HashSet<int> _postTriggers;

        UnityEvent<Trigger> _onTriggerEnter;
        public UnityEvent<Trigger> OnTriggerEnter => _onTriggerEnter ??= new();
         UnityEvent<Trigger> _onTriggerExit;
        public UnityEvent<Trigger> OnTriggerExit => _onTriggerExit ??= new();
        public void Init()
        {
            _postTriggers = new ();
            GameManager.instance.WhenReturnedToTitle.RemoveListener(ClearTriggers);
            GameManager.instance.WhenReturnedToTitle.AddListener(ClearTriggers);
        }

        public bool CheckActivated(int triggerId)
        {
            return _postTriggers.Contains(triggerId);
        }
        public void ClearTriggers()
        {
            _postTriggers.Clear();
        }

        public void ActivateTrigger(int triggerId)
        {
            _postTriggers.Add(triggerId);
        }
    }
}