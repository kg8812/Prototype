using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Apis
{
    public abstract class IObjectFactory
    {
        static AddressablePooling pool;
        
        protected static AddressablePooling Pool
        {
            get
            {
                pool ??= new ("Object");
                return pool;
            }
        }

        private readonly Dictionary<GameObject, Sequence> returnSequences = new();

        public abstract GameObject Get(string addressName, Vector2? pos = null);

        public Sequence Return(GameObject target, float time = 0) // 반환 함수
        {
            if (returnSequences.TryGetValue(target, out var sq))
            {
                sq?.Kill();
                returnSequences.Remove(target);
            }

            if (Mathf.Approximately(time,0))
            {
                ReturnTarget(target);
                return null;
            }
            
            Sequence seq = DOTween.Sequence();

            returnSequences.Add(target,seq);
            
            seq.AppendInterval(time);
            seq.AppendCallback(() =>
            {
                ReturnTarget(target);
                returnSequences.Remove(target);
            });

            return seq;
        }

        void ReturnTarget(GameObject target)
        {
            if (target.TryGetComponent(out BoneFollower boneFollower))
            {
                Object.Destroy(boneFollower);
            }
            
            Pool.Return(target);
        }
    }
}