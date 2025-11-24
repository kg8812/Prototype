using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis;
using NewNewInvenSpace;
using UnityEngine;

public class UI_CheatAccessoryPage : UI_CheatItemPage
{
    protected override List<Item> GetWholeItemList()
    {
        return GameManager.Item.Accessories.OrderBy(x => x.ItemId).Select(x => x as Item).ToList();
    }

    protected override void AddItem(Item item)
    {
        if (InvenManager.instance.Acc.IsFull(InvenType.Storage))
        {
            var pickUp = GameManager.Item.AccPickUp.CreateNew(item.ItemId);
            pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
        }
        else
        {
            var acc = GameManager.Item.GetAcc(item.ItemId);
            InvenManager.instance.Acc.Add(acc, InvenType.Storage);
        }
    }
}
