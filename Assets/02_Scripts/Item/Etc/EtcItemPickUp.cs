using System.Collections;
using System.Collections.Generic;
using Apis;
using NewNewInvenSpace;
using UI;
using UnityEngine;

public class EtcItemPickUp : DropItem
{
    public int itemId;
    // public string itemName;
    EtcItem item; // 획득할 아이템
    SpriteRenderer render;

    private void Awake()
    {
        isInteractable = true;
        render = GetComponentInChildren<SpriteRenderer>();
    }
    
    public override void InvokeInteraction()
    {
        if (item == null)
        {
            item = GameManager.Item.GetEtcItem(itemId);
        }
        InvenManager.instance.GuitarInven.Add(item,GuitarInvenType.Growth);
        Destroy(gameObject);
    }
}
