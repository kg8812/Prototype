using System.Collections.Generic;
using Apis;

namespace Apis
{
    public class Factory_Germ: ItemFactory<Germ>
    {
        private readonly IDictionary<int, Germ> _germDict;
        
        public Factory_Germ(Germ[] objs) : base(objs)
        {
            _germDict = new Dictionary<int, Germ>();
            
            foreach (var x in objs)
            {
                _germDict.TryAdd(x.germId, x);
            }
        }

        public override Germ CreateNew(int id)
        {
            if (_germDict.TryGetValue(id, out var value))
            {
                return pool.Get(value.name);
            }
            return null;
        }

        public override Germ CreateRandom()
        {
            return null;
        }

        public override List<Germ> CreateAll()
        {
            return null;
        }
    }
}