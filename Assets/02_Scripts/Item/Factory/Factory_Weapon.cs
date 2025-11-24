using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apis
{
    public class Factory_Weapon : ItemFactory<Weapon>
    {
        readonly Dictionary<int, Weapon> wpDict = new Dictionary<int, Weapon>();
        public Dictionary<int, Weapon> WpDict => wpDict;
        public Factory_Weapon(Weapon[] objs) : base(objs)
        {
            foreach (var x in objs)
            {
                wpDict.TryAdd(x.ItemId, x);
            }
        }

        public override Weapon CreateNew(int itemId)
        {
            // Name = Name.Replace(" ", "");
            
            if (wpDict.TryGetValue(itemId, out var value))
            {
                Weapon weapon = pool.Get(value.name);
                weapon.Init();
                return weapon;
            }

            return null;
        }
        
        public override Weapon CreateRandom()
        {
            int rand = Random.Range(0, wpDict.Count);
            Weapon weapon = pool.Get(wpDict.ElementAt(rand).Value.name);
            weapon.Init();
            return weapon;
        }

        public override List<Weapon> CreateAll()
        {
            List<Weapon> list = new();

            foreach (Weapon wp in wpDict.Values)
            {
                Weapon weapon = pool.Get(wp.name);

                weapon.Init();
                list.Add(weapon);
            }

            return list;
        }
    }
}