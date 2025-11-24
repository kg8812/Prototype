using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{

    public class ItemDropper : MonoBehaviour
    {
        public UnityEvent whenDropped;
        [SerializeField] private int dropperId;

        public int DropperId
        {
            get
            {
                return dropperId;
            }
            set
            {
                dropperId = value;
            }
        }
        
        public List<DropItem> Drop()
        {
            if (DropperId == 0) return null;
            
            List<DropItem> dropItems = GameManager.Drop.GetDropItems(DropperId);
            foreach (var dropItem in dropItems)
            {
                dropItem.transform.position = gameObject.transform.position;
            }
            whenDropped.Invoke();

            return dropItems;
        }
    }

}
