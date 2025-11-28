using Apis;
using Default;
using UnityEngine;

public class AttackObjectFactory : IObjectFactory
{
    public override GameObject Get(string objectName, Vector2? pos = null)
    {
        var obj = Pool.Get("Prefabs/AttackObjects/" + objectName, pos);
        if (obj == null) return null;

        Utils.GetOrAddComponent<AttackObject>(obj);
        obj.transform.rotation = Quaternion.identity;
        if (obj.TryGetComponent(out ParticleSystem effect))
        {
            effect.Simulate(0, true, true);
            effect.Play();
        }

        return obj;
    }
}