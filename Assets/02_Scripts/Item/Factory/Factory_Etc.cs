using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using UnityEngine;

public class Factory_Etc : ItemFactory<EtcItem>
{
    readonly Dictionary<int, EtcItem> dict;
    public Dictionary<int, EtcItem> Dict => dict;
    public Factory_Etc(EtcItem[] objs) : base(objs)
    {
        dict = new Dictionary<int, EtcItem>();
        foreach (var x in objs)
        {
            if (!dict.ContainsKey(x.ItemId))
            {
                dict.Add(x.ItemId, x);
            }
        }
    }

    public override EtcItem CreateNew(int itemId)
    {
        // itemName = itemName.Replace(" ", "");
        if (Dict.TryGetValue(itemId, out var item))
        {
            EtcItem etcItem = pool.Get(item.name);
            etcItem.Init();
            return etcItem;
        }
        return null;
    }

    public override EtcItem CreateRandom()
    {
        var list = Dict.Values.ToList();
        int rand = Random.Range(0, list.Count);
        EtcItem etcItem = pool.Get(list[rand].name);
        etcItem.Init();
        return etcItem;
    }

    public override List<EtcItem> CreateAll()
    {
        List<EtcItem> list = new();

        foreach (var name in Dict.Keys)
        {
            EtcItem etc = pool.Get(Dict[name].name);
            etc.Init();
            list.Add(etc);
        }

        return list;
    }
}
