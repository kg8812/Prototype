using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis;
using Default;
using NewNewInvenSpace;
using UnityEngine;

public class UI_CheatWeaponPage : UI_CheatItemPage
{
    protected override List<Item> GetWholeItemList()
    {
        return GameManager.Item.WeaponList.OrderBy(x => x.Index).Select(x => x as Item).ToList();
    }

    protected override void AddItem(Item item)
    {
        if (InvenManager.instance.AttackItem.IsFull(InvenType.Storage))
        {
            var pickUp = GameManager.Item.WeaponPickUp.CreateNew(item.ItemId);
            pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
        }
        else
        {
            var wp = GameManager.Item.GetWeapon(item.ItemId);
            InvenManager.instance.AttackItem.Add(wp, InvenType.Storage);
        }
    }
}
