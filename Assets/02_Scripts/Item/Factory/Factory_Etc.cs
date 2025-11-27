using System.Collections.Generic;
using System.Linq;
using Apis;
using UnityEngine;

public class Factory_Etc : ItemFactory<EtcItem>
{
    public Factory_Etc(EtcItem[] objs) : base(objs)
    {
        Dict = new Dictionary<int, EtcItem>();
        foreach (var x in objs)
            if (!Dict.ContainsKey(x.ItemId))
                Dict.Add(x.ItemId, x);
    }

    public Dictionary<int, EtcItem> Dict { get; }

    public override EtcItem CreateNew(int itemId)
    {
        // itemName = itemName.Replace(" ", "");
        if (Dict.TryGetValue(itemId, out var item))
        {
            var etcItem = pool.Get(item.name);
            etcItem.Init();
            return etcItem;
        }

        return null;
    }

    public override EtcItem CreateRandom()
    {
        var list = Dict.Values.ToList();
        var rand = Random.Range(0, list.Count);
        var etcItem = pool.Get(list[rand].name);
        etcItem.Init();
        return etcItem;
    }

    public override List<EtcItem> CreateAll()
    {
        List<EtcItem> list = new();

        foreach (var name in Dict.Keys)
        {
            var etc = pool.Get(Dict[name].name);
            etc.Init();
            list.Add(etc);
        }

        return list;
    }
}