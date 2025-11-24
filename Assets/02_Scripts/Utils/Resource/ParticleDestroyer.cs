using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ParticleDestroyer : MonoBehaviour, IPoolObject
{
    private UnityEvent _onDestroy;

    public UnityEvent OnDestroyed => _onDestroy ??= new();

    [LabelText("지워야하는 목록")] public List<ParticleSystem> removes = new();
    [LabelText("남겨야하는 목록")] public List<ParticleSystem> remains = new();
    [LabelText("멈춰야되는 목록")] public List<ParticleSystem> stopAndRemain = new();
    [LabelText("멈춘 후 삭제되는 시간")] public float remainingTime;

    Renderer[] _renderers;
    public bool disappearWhenHide;
    private ParticleSystem[] _particles;
    
    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(true).Where(x => x.gameObject != gameObject).ToArray();
        _particles = GetComponentsInChildren<ParticleSystem>(true);
        var exceptSet = new HashSet<ParticleSystem>(removes.Concat(remains).Concat(stopAndRemain));
        _particles = _particles.Where(p => !exceptSet.Contains(p)).ToArray();
    }

    public bool CheckRemoveLists()
    {
        return removes is { Count: > 0 } || remains is { Count: > 0 } || stopAndRemain is { Count: > 0 };
    }

    public void StopEmitting()
    {
        foreach (var particle in _particles)
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        removes.ForEach(x =>
        {
            if (x == null) return;
            x.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear); 
        });
        remains.ForEach(x =>
        {
            if (x == null) return;
            x.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        });
        stopAndRemain.ForEach(x =>
        {
            if (x == null) return;
            x.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[x.particleCount];
            int count = x.GetParticles(particles);
            for (int i = 0; i < count; i++)
            {
                particles[i].velocity = Vector2.zero;
                particles[i].remainingLifetime = remainingTime;
            }

            x.SetParticles(particles, count);
        });
    }

    private void OnParticleSystemStopped()
    {
        Return();
    }

    public void Return()
    {
        OnDestroyed.Invoke();
        OnDestroyed.RemoveAllListeners();
        GameManager.Factory.Return(gameObject);
    }

    public void TurnRenderers(bool isOn)
    {
        foreach (var renderer1 in _renderers)
        {
            if (renderer1 == null) continue;
            renderer1.enabled = isOn;
        }
    }
    public void OnGet()
    {
        _renderers = GetComponentsInChildren<Renderer>(true).Where(x => x.gameObject != gameObject).ToArray();

        TurnRenderers(true);
    }

    public void OnReturn()
    {
        OnDestroyed.RemoveAllListeners();
    }
}