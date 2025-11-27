using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public abstract partial class Skill
    {
        private EffectSpawner effectSpawner;

        public void SpawnEffectInPos(string enter, string loop, float radius, Vector2 pos, bool spawnAfterEnter = false,
            UnityAction<ParticleDestroyer> WhenLoopSpawn = null)
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return;
            }

            var particle = effectSpawner.Spawn(enter, pos, false);


            particle.transform.localScale = Vector3.one * (radius * 2);

            if (spawnAfterEnter)
            {
                particle.OnDestroyed.AddListener(() =>
                {
                    var ef = effectSpawner.Spawn(loop, pos, false);

                    ef.transform.localScale = Vector3.one * (radius * 2);

                    WhenLoopSpawn?.Invoke(ef);
                });
            }
            else
            {
                var ef = effectSpawner.Spawn(loop, pos, false);

                ef.transform.localScale = Vector3.one * (radius * 2);

                WhenLoopSpawn?.Invoke(ef);
            }
        }

        public ParticleDestroyer SpawnEffect(string address, float radius, Vector2 pos, bool disappearWhenHide)
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return null;
            }

            var ef = effectSpawner.Spawn(address, pos, false, disappearWhenHide);

            ef.transform.localScale = Vector3.one * (radius * 2);
            return ef;
        }

        public void RemoveEffect(string address)
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return;
            }

            effectSpawner.Remove(address);
        }

        public void RemoveAllEffects()
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return;
            }

            effectSpawner.RemoveAllEffects();
        }
    }
}