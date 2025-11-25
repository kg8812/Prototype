using System.Collections.Generic;
using System.Linq;
using Save.Schema;
using UnityEngine;

namespace Apis
{
    public class Factory_Acc : ItemFactory<Accessory>
    {
        //악세사리 팩토리

        readonly Dictionary<int, Accessory> accDict;
        public Dictionary<int, Accessory> AccDict => accDict;
        public Factory_Acc(Accessory[] objs) : base(objs)
        {
            accDict = new Dictionary<int, Accessory>();
            foreach (var x in objs)
            {
                accDict.TryAdd(x.ItemId, x);
            }
        }

        public override List<Accessory> CreateAll()
        {
            List<Accessory> list = new();

            foreach (var name in accDict.Keys)
            {
                Accessory accessory = pool.Get(accDict[name].name);
                accessory.Init();
                list.Add(accessory);
            }

            return list;
        }

        public override Accessory CreateNew(int itemId)
        {
            if (accDict.TryGetValue(itemId, out var item))
            {
                Accessory acc = pool.Get(item.name);
                acc.Init();
                return acc;
            }
            return null;
        }

        public override Accessory CreateRandom()
        {
            var list = accDict.Values.ToList();
            int rand = Random.Range(0, list.Count);
            Accessory acc = pool.Get(list[rand].name);
            acc.Init();
            return acc;
        }
    }
}