using System;
using Apis;
using UnityEngine;

public class AfterEffectSpawner : MonoBehaviour, IPoolObject
{
    public string effectName;
    public bool useDuration;

    private EffectSpawner _spawner;

    /// 추가 이펙트 소환시 호출되는 이벤트
    public Action<GameObject> OnSpawn;

    private Transform parent;

    public void OnGet()
    {
        OnSpawn = null;
        if (useDuration) Invoke(nameof(Spawn), GetComponent<ParticleSystem>().main.duration);
    }

    public void OnReturn()
    {
        if (!useDuration) Spawn();
    }

    public void Init(EffectSpawner spawner)
    {
        _spawner = spawner;
    }

    private void Spawn()
    {
        parent = transform.parent;

        var obj = _spawner != null
            ? _spawner.Spawn(effectName, transform.position, false).gameObject
            : GameManager.Factory.Get(FactoryManager.FactoryType.Effect, effectName, transform.position);

        OnSpawn?.Invoke(obj);
        if (parent != null)
        {
            obj.transform.SetParent(parent, true);
            obj.transform.localPosition = transform.localPosition;
        }

        obj.transform.localScale = transform.localScale;
    }
}