using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Apis
{
    public class FactoryManager
    {
        public enum Direction
        {
            Vertical,
            Horizontal
        }

        public enum FactoryType
        {
            Normal,
            AttackObject,
            Effect,
            Monster
        }

        public readonly Dictionary<GameObject, Sequence> destroyList = new();

        private Dictionary<FactoryType, IObjectFactory> _factories;

        private IObjectFactory _factory = new ObjectFactory();

        private Dictionary<FactoryType, IObjectFactory> Factories
        {
            get
            {
                if (_factories == null)
                {
                    _factories = new Dictionary<FactoryType, IObjectFactory>();
                    _factories.Add(FactoryType.Normal, new ObjectFactory());
                    _factories.Add(FactoryType.AttackObject, new AttackObjectFactory());
                    _factories.Add(FactoryType.Effect, new EffectFactory());
                    _factories.Add(FactoryType.Monster, new MonsterFactory());
                }

                return _factories;
            }
        }

        public Sequence Return(GameObject obj, float duration = 0, TweenCallback afterReturn = null)
        {
            if (obj == null) return null;
            var seq = _factory.Return(obj, duration);
            if (seq == null) return null;

            if (afterReturn != null && seq.IsActive()) seq.onComplete += afterReturn;

            if (destroyList.TryAdd(obj, seq))
            {
                seq.onKill += () => { destroyList.Remove(obj); };
            }
            else
            {
                seq.Kill();
                return seq;
            }

            return seq;
        }

        public GameObject Get(FactoryType type, string address, Vector2? pos = null)
        {
            _factory = Factories[type];
            return _factory?.Get(address, pos);
        }

        public T Get<T>(FactoryType type, string address, Vector2? pos = null) where T : Component
        {
            _factory = Factories[type];
            return _factory?.Get(address, pos).GetComponent<T>();
        }


        public List<T> SpawnWithPadding<T>(FactoryType type, string address, Vector2 basePos, int spawnCount,
            Direction dir,
            float padding = 0.5f) where T : Component
        {
            List<T> objects = new();
            for (var i = 1; i <= spawnCount; i++)
            {
                var obj = Get<T>(type, address,
                    basePos + (dir == Direction.Horizontal ? Vector2.right : Vector2.up) *
                    (padding * Mathf.Pow(-1, i) * ((i + 1) / 2)));
                objects.Add(obj);
            }

            return objects;
        }

        public List<Projectile> SpawnProjectilesInCircle(string address, Vector2 basePos, int spawnCount, float radius)
        {
            List<Projectile> list = new();
            if (spawnCount == 0) return list;
            var angle = 360f / spawnCount;

            for (var i = 0; i < spawnCount; i++)
            {
                var rad = angle * i * Mathf.Deg2Rad;
                var sin = Mathf.Sin(rad);
                var cos = Mathf.Cos(rad);
                var projectile = Get<Projectile>(FactoryType.AttackObject, address,
                    basePos + new Vector2(sin, cos) * radius);
                list.Add(projectile);
            }

            return list;
        }
    }
}