using UnityEngine;

namespace Apis.Components
{
    public class TA_SpawnComponent: ITriggerActivate
    {
        private Trigger _triggerComponent;
        private GameObject _componentGroup;
        
        public TA_SpawnComponent(Trigger triggerComponent, GameObject componentGroup)
        {
            _triggerComponent = triggerComponent;
            _componentGroup = componentGroup;
        }
        
        public void Activate()
        {
            if (_componentGroup == null)
            {
                Debug.LogWarning("Trigger component group is null");
            }
            _componentGroup?.SetActive(true);
        }
    }
}