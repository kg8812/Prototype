using Apis;
using Default;
using Unity.Mathematics;
using UnityEngine;

public class EffectFactory : IObjectFactory
{
    public override GameObject Get(string addressName, Vector2? pos = null)
    {
        var obj = Pool.Get(addressName, pos);
        if (obj == null) return null;

        obj.transform.rotation = quaternion.identity;
        obj.GetOrAddComponent<ParticleDestroyer>();
        Utils.ActionAfterFrame(() =>
        {
            if (obj.TryGetComponent(out ParticleSystem effect))
            {
                effect.Simulate(0, true, true);
                effect.Play();
            }
        });

        return obj;
    }
}