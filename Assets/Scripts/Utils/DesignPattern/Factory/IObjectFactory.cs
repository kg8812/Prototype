using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Apis
{
    public abstract class IObjectFactory
    {
        private static AddressablePooling pool;

        private readonly Dictionary<GameObject, Sequence> returnSequences = new();

        protected static AddressablePooling Pool
        {
            get
            {
                pool ??= new AddressablePooling("Object");
                return pool;
            }
        }

        /// <summary>로딩 단계에서 미리 인스턴스를 만들어 둔다.</summary>
        public static void PrewarmPool(string address, int count)
        {
            Pool.Prewarm(address, count);
        }

        /// <summary>씬 전환 시 게임플레이 오브젝트를 모두 파기한다.</summary>
        public static void ClearPool()
        {
            pool?.Clear();
        }

        public abstract GameObject Get(string addressName, Vector2? pos = null);

        public Sequence Return(GameObject target, float time = 0) // 반환 함수
        {
            if (returnSequences.TryGetValue(target, out var sq))
            {
                sq?.Kill();
                returnSequences.Remove(target);
            }

            if (Mathf.Approximately(time, 0))
            {
                ReturnTarget(target);
                return null;
            }

            var seq = DOTween.Sequence();

            returnSequences.Add(target, seq);

            seq.AppendInterval(time);
            seq.AppendCallback(() =>
            {
                ReturnTarget(target);
                returnSequences.Remove(target);
            });

            return seq;
        }

        private void ReturnTarget(GameObject target)
        {
            if (target.TryGetComponent(out BoneFollower boneFollower)) Object.Destroy(boneFollower);

            Pool.Return(target);
        }
    }
}