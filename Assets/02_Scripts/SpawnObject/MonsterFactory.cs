using Apis;
using UnityEngine;

namespace Apis
{
    public class MonsterFactory: IObjectFactory
    {
        public override GameObject Get(string addressName, Vector2? pos = null)
        {
            if (!addressName.Contains("Prefabs/Components/MonsterComponent/"))
            {
                addressName = "Prefabs/Components/MonsterComponent/" + addressName;
            }
            
            GameObject obj = Pool.Get(addressName,pos);
            if (obj == null) return null;
            if (obj.TryGetComponent(out Monster monster))
            {
                // Debug.Log($"monster Init {monster.gameObject.name}");
                // i pool object.OnGet()에서 해줌
            }
            return obj;
        }
    }
}