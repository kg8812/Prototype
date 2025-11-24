using UnityEngine;

namespace Apis.Components
{
    public class TS_PlayerExit: ITriggerStrategy
    {
        private Trigger _triggerComponent;

        public TS_PlayerExit(Trigger triggerComponent)
        {
            _triggerComponent = triggerComponent;
        }
        
        public void Update()
        {
            
        }

        public void OnTriggerEnter2D(Collider2D col)
        {
            
        }

        public void OnTriggerExit2D(Collider2D col)
        {
            _triggerComponent?.ActivateTrigger();
        }

        public bool CheckAvailable(Collider2D col)
        {
            return col.gameObject.CompareTag("Player") && !col.isTrigger;
        }
    }
}