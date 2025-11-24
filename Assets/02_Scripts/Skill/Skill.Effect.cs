using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

namespace Apis
{
    public abstract partial class Skill
    {
        private EffectSpawner effectSpawner;

        public void SpawnEffect(string enter, string loop, float radius, bool spawnAfterEnter = false, UnityAction<ParticleDestroyer> WhenLoopSpawn = null,string boneName = "center")
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return;
            }
            Sequence seq = DOTween.Sequence();
            
            var particle = effectSpawner.Spawn(enter, boneName,false).GetComponent<ParticleSystem>();

            particle.transform.localScale = Vector3.one * (radius * 2);
            
            if (spawnAfterEnter)
            {
                seq.AppendInterval(particle.main.duration);
                seq.AppendCallback(() =>
                {
                    var ef = effectSpawner.Spawn(loop, boneName,false); 
                    
                    ef.transform.localScale = Vector3.one * (radius * 2);

                    WhenLoopSpawn?.Invoke(ef);
                });
            }
            else
            {
                var ef = effectSpawner.Spawn(loop, boneName,false); 
                
                ef.transform.localScale = Vector3.one * (radius * 2);

                WhenLoopSpawn?.Invoke(ef);
            }

        }

        public void SpawnEffectInPos(string enter, string loop, float radius,Vector2 pos, bool spawnAfterEnter = false,
            UnityAction<ParticleDestroyer> WhenLoopSpawn = null)
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return;
            }
            
            var particle = effectSpawner.Spawn(enter, pos,false);

            
            particle.transform.localScale = Vector3.one * (radius * 2);
            
            if (spawnAfterEnter)
            {
                particle.OnDestroyed.AddListener(() =>
                {
                    var ef = effectSpawner.Spawn(loop, pos,false); 
                    
                    ef.transform.localScale = Vector3.one * (radius * 2);

                    WhenLoopSpawn?.Invoke(ef);
                });
            }
            else
            {
                var ef = effectSpawner.Spawn(loop, pos,false); 
                
                ef.transform.localScale = Vector3.one * (radius * 2);

                WhenLoopSpawn?.Invoke(ef);
            }
        }

        public ParticleDestroyer SpawnEffect(string address,float radius,bool disappearWhenHide,string boneName = "center")
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return null;
            }
            var ef = effectSpawner.Spawn(address, boneName,false,disappearWhenHide); 
           
            ef.transform.localScale = Vector3.one * (radius * 2);

            return ef;
        }
        
        public ParticleDestroyer SpawnEffect(string address, float radius,Vector2 pos,bool disappearWhenHide)
        {
            if (effectSpawner == null)
            {
                Debug.LogError("effectSpawner cannot be null");
                return null;
            }
            ParticleDestroyer ef = effectSpawner.Spawn(address,pos,false,disappearWhenHide);
            
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