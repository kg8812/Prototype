using Apis;
using Default;
using UnityEngine;

public class ParticleTrailFollower : MonoBehaviour
{
    public string trailAddress;
    private Projectile projectile;

    private ParticleSystem trail;

    private void Awake()
    {
        projectile = Utils.GetComponentInParentAndChild<Projectile>(gameObject);
        projectile.AddEvent(EventType.OnInit, SpawnTrail);
        projectile.AddEvent(EventType.OnDestroy, RemoveTrail);
    }

    private void Update()
    {
        trail.transform.position = transform.position;
    }

    private void SpawnTrail(EventParameters parameters)
    {
        trail = GameManager.Factory.Get<ParticleSystem>(FactoryManager.FactoryType.Effect, trailAddress,
            transform.position);
        trail.transform.localScale = projectile.transform.localScale;
    }

    private void RemoveTrail(EventParameters parameters)
    {
        trail.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        GameManager.Factory.Return(trail.gameObject, 1);
    }
}