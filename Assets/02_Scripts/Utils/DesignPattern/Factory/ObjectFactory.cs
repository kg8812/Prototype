using UnityEngine;

namespace Apis
{
    public class ObjectFactory : IObjectFactory
    {
        public override GameObject Get(string addressName, Vector2? pos = null)
        {
            GameObject obj = Pool.Get(addressName, pos);
            if (obj == null) return null;

            return obj;
        }
    }
}