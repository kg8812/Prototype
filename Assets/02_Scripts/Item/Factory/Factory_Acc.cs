using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apis
{
    public class Factory_Acc : ItemFactory<Accessory>
    {
        //악세사리 팩토리

        public Factory_Acc(Accessory[] objs) : base(objs)
        {
            AccDict = new Dictionary<int, Accessory>();
            foreach (var x in objs) AccDict.TryAdd(x.ItemId, x);
        }

        public Dictionary<int, Accessory> AccDict { get; }

        public override List<Accessory> CreateAll()
        {
            List<Accessory> list = new();

            foreach (var name in AccDict.Keys)
            {
                var accessory = pool.Get(AccDict[name].name);
                accessory.Init();
                list.Add(accessory);
            }

            return list;
        }

        public override Accessory CreateNew(int itemId)
        {
            if (AccDict.TryGetValue(itemId, out var item))
            {
                var acc = pool.Get(item.name);
                acc.Init();
                return acc;
            }

            return null;
        }

        public override Accessory CreateRandom()
        {
            var list = AccDict.Values.ToList();
            var rand = Random.Range(0, list.Count);
            var acc = pool.Get(list[rand].name);
            acc.Init();
            return acc;
        }
    }
}