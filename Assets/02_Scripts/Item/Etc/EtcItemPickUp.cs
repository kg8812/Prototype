using Apis;
using NewNewInvenSpace;
using UnityEngine;

public class EtcItemPickUp : DropItem
{
    public int itemId;

    // public string itemName;
    private EtcItem item; // 획득할 아이템
    private SpriteRenderer render;

    private void Awake()
    {
        isInteractable = true;
        render = GetComponentInChildren<SpriteRenderer>();
    }

    public override void InvokeInteraction()
    {
        if (item == null) item = GameManager.Item.GetEtcItem(itemId);
        InvenManager.instance.GuitarInven.Add(item, GuitarInvenType.Growth);
        Destroy(gameObject);
    }
}