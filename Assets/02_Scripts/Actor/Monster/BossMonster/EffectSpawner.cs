using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Default;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Apis
{
    public class EffectSpawner
    {
        private readonly Dictionary<string, List<ParticleDestroyer>> spawnedEffects = new();
        List<ParticleDestroyer> allEffects = new();
        private readonly Actor _user;

        public EffectSpawner(Actor user)
        {
            _user = user;
            user?.OnHide.AddListener(() =>
            {
                TurnEffectsOnOrOff(false);
            });
            user?.OnAppear.AddListener(() =>
            {
                TurnEffectsOnOrOff(true);
            });
            user?.AddEvent(EventType.OnEnable, _ =>
            {
                TurnEffectsOnOrOff(true);
            });
            user?.AddEvent(EventType.OnDisable, _ =>
            {
                TurnEffectsOnOrOff(false);
            });
            
            user?.AddEvent(EventType.OnDestroy, _ => ReturnAllEffects());
        }

        ParticleDestroyer SpawnObject(string address, Vector2 position,bool setEffectParent,bool disappearWhenHide)
        {
            if (_user == null) return null;
            var vfx = GameManager.Factory.Get(FactoryManager.FactoryType.Effect, address, position);
            
            if (!spawnedEffects.ContainsKey(address))
            {
                spawnedEffects.Add(address,new());
            }
            
            var destroyer = vfx.GetOrAddComponent<ParticleDestroyer>();
            spawnedEffects[address].Add(destroyer);
            allEffects.Add(destroyer);
            destroyer.OnDestroyed.AddListener(() =>
            {
                spawnedEffects[address].Remove(destroyer);
                allEffects.Remove(destroyer);
            });
            destroyer.disappearWhenHide = disappearWhenHide;
            if (setEffectParent)
            {
                _user?.SetEffectParent(vfx);
            }
            return destroyer;
        }
        /// <summary>
        /// 이펙트 생성
        /// </summary>
        /// <param name="address">주소</param>
        /// <param name="position">생성 위치</param>
        public ParticleDestroyer Spawn(string address, Vector2 position, bool setEffectParent,bool disappearWhenHide = false)
        {
            return SpawnObject(address, position,setEffectParent,disappearWhenHide);
        }

        /// <summary>
        /// 이펙트 생성
        /// </summary>
        /// <param name="address">주소</param>
        /// <param name="boneName">고정시킬 Bone Name</param>
        public ParticleDestroyer Spawn(string address, string boneName, bool setEffectParent,bool disappearWhenHide = false)
        {
            if (_user == null) return null;
            var vfx = SpawnObject(address, _user.Position, setEffectParent, disappearWhenHide);

            if (vfx != null && _user.ActorRenderer is SpineRenderer spineRenderer)
            {
                SpineUtils.AddBoneFollower(spineRenderer.Mecanim, boneName, vfx.gameObject);
            }

            return vfx;
        }

        public ParticleDestroyer Spawn(string address, string boneName, Vector2 offset, bool setEffectParent, bool disappearWhenHide = false)
        {
            if (_user == null) return null;

            var vfx = SpawnObject(address, _user.Position, setEffectParent, disappearWhenHide);
            if (vfx != null &&  _user.ActorRenderer is SpineRenderer spineRenderer)
            {
                var follower = SpineUtils.AddCustomBoneFollower(spineRenderer.Mecanim, boneName, vfx.gameObject);
                follower.offset = offset;
            }

            return vfx;
        }

        /// <summary>
        /// 이펙트 생성, 지속시간 후 Destroy
        /// </summary>
        /// <param name="address">주소</param>
        /// <param name="boneName">고정시킬 Bone 이름</param>
        /// <param name="afterDestroy">Destroy 후 실행시킬 함수</param>
        public ParticleDestroyer SpawnAndDestroyAfterDuration(string address, string boneName, UnityAction afterDestroy, bool setEffectParent,bool disappearWhenHide = false)
        {
            if (_user == null) return null;

            var vfx = SpawnObject(address, _user.Position, setEffectParent, disappearWhenHide);

            if (vfx != null &&  _user.ActorRenderer is SpineRenderer spineRenderer)
            {
                SpineUtils.AddCustomBoneFollower(spineRenderer.Mecanim, boneName, vfx.gameObject);
                vfx.OnDestroyed.AddListener(afterDestroy);
            }

            return vfx;
        }

        /// <summary>
        /// 이펙트 생성, 지속시간 후 Destroy
        /// </summary>
        /// <param name="address">주소</param>
        /// <param name="position"> 소환 위치</param>
        /// <param name="afterDestroy">Destroy 후 실행시킬 함수</param>
        public ParticleDestroyer SpawnAndDestroyAfterDuration(string address, Vector2 position, UnityAction afterDestroy, bool setEffectParent,bool disappearWhenHide = false)
        {
            if (_user == null) return null;
            
            var vfx = SpawnObject(address, position, setEffectParent, disappearWhenHide);
            
            vfx?.OnDestroyed.AddListener(afterDestroy);
           
            return vfx;
        }

        /// <summary>
        /// 소환중인 이펙트 제거
        /// </summary>
        /// <param name="address">소환한 이펙트 주소</param>
        public void Remove(string address)
        {
            if (spawnedEffects.ContainsKey(address))
            {
                List<ParticleDestroyer> effects = spawnedEffects[address].ToList();
                spawnedEffects[address].Clear();
                allEffects = allEffects.Except(effects).ToList();
                effects.ForEach(effect =>
                {
                    effect.StopEmitting();
                });

            }
        }

        /// <summary>
        /// 소환중인 이펙트 제거
        /// </summary>
        /// <param name="effect">이펙트</param>
        /// <returns></returns>
        public void Remove(ParticleDestroyer effect)
        {
            spawnedEffects.Values.ForEach(x =>
            {
                x.Remove(effect);
            });
            allEffects.Remove(effect);
            effect.StopEmitting();
        }

        /// <summary>
        /// 오브젝트가 소환한 이펙트 전부 제거
        /// </summary>
        public void RemoveAllEffects()
        {
            spawnedEffects.Values.ForEach(effects =>
            {
                var temp = effects.ToList();
                temp.ForEach(effect =>
                {
                   effect.StopEmitting();
                });
            });
            allEffects.Clear();
        }

        void ReturnAllEffects()
        {
            if (GameManager.IsQuitting) return;
            
            spawnedEffects.Values.ForEach(effects =>
            {
                var temp = effects.ToList();
                temp.ForEach(effect =>
                {
                    effect.Return();
                });
            });
            allEffects.Clear();
            spawnedEffects.Clear();
        }
        public void TurnEffectsOnOrOff(bool isOn)
        {
            allEffects.ForEach(x =>
            {
                if (!x.disappearWhenHide) return;
                x.TurnRenderers(isOn);
            });
        }
    }
}