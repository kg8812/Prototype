using UnityEngine;

namespace Apis.Components
{
    public class TS_PlayerEnter: ITriggerStrategy
    {
        private Trigger _triggerComponent;

        public TS_PlayerEnter(Trigger triggerComponent)
        {
            _triggerComponent = triggerComponent;
        }
        public void OnTriggerEnter2D(Collider2D col)
        {
            _triggerComponent.ActivateTrigger();
        }

        public void OnTriggerExit2D(Collider2D col)
        {
            
        }

        public bool CheckAvailable(Collider2D col)
        {
            return col.gameObject.CompareTag("Player") && !col.isTrigger;
        }
    }
}