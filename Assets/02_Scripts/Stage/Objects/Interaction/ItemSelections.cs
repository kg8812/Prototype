using System.Collections.Generic;
using Apis;
using UnityEngine;
using UnityEngine.Events;

public class ItemSelections : SingleUseInteraction
{
    public UnityEvent whenDropped;
    [SerializeField] private int dropperId;

    public int DropperId
    {
        get => dropperId;
        set => dropperId = value;
    }
        
    public List<DropItem> Drop()
    {
        if (DropperId == 0) return null;
            
        List<DropItem> dropItems = GameManager.Drop.GetDropItems(DropperId);

        int count = dropItems.Count;
        float padding = 1.5f;
        float length = (count - 1) * padding;
        
        for (int i = 0; i < count; i++)
        {
            dropItems[i].rigid.velocity = Vector2.zero;
            dropItems[i].transform.position = transform.position + Vector3.right * (-length / 2 + i * padding);
        }
        whenDropped.Invoke();

        return dropItems;
    }
}
