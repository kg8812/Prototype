using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apis
{
    public class Factory_Weapon : ItemFactory<Weapon>
    {
        public Factory_Weapon(Weapon[] objs) : base(objs)
        {
            foreach (var x in objs) WpDict.TryAdd(x.ItemId, x);
        }

        public Dictionary<int, Weapon> WpDict { get; } = new();

        public override Weapon CreateNew(int itemId)
        {
            // Name = Name.Replace(" ", "");

            if (WpDict.TryGetValue(itemId, out var value))
            {
                var weapon = pool.Get(value.name);
                weapon.Init();
                return weapon;
            }

            return null;
        }

        public override Weapon CreateRandom()
        {
            var rand = Random.Range(0, WpDict.Count);
            var weapon = pool.Get(WpDict.ElementAt(rand).Value.name);
            weapon.Init();
            return weapon;
        }

        public override List<Weapon> CreateAll()
        {
            List<Weapon> list = new();

            foreach (var wp in WpDict.Values)
            {
                var weapon = pool.Get(wp.name);

                weapon.Init();
                list.Add(weapon);
            }

            return list;
        }
    }
}