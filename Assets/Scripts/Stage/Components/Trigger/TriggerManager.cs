using System.Collections.Generic;
using UnityEngine.Events;

namespace Apis.Components
{
    public class TriggerManager
    {
        private UnityEvent<Trigger> _onTriggerEnter;
        private UnityEvent<Trigger> _onTriggerExit;
        private HashSet<int> _postTriggers;
        public UnityEvent<Trigger> OnTriggerEnter => _onTriggerEnter ??= new UnityEvent<Trigger>();
        public UnityEvent<Trigger> OnTriggerExit => _onTriggerExit ??= new UnityEvent<Trigger>();

        public void Init()
        {
            _postTriggers = new HashSet<int>();
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