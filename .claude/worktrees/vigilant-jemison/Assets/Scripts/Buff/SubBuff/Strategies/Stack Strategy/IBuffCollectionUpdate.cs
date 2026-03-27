using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public interface IBuffCollectionUpdate : IObserver<List<SubBuff>>
    {
        public float Duration { get; set; }
        public float CurTime { get; set; }
        public void Update();
        public void ResetTime();
    }

    public class SingleStackDecrease : IBuffCollectionUpdate
    {
        private readonly SubBuffCollection collection;
        private List<SubBuff> list;

        public SingleStackDecrease(SubBuffCollection collection, float duration)
        {
            this.collection = collection;
            list = collection.List;
            Duration = duration;
            CurTime = duration;
        }

        public float Duration { get; set; }

        public float CurTime { get; set; }

        public void Notify(List<SubBuff> value)
        {
            list = value;
        }

        public void Update()
        {
            if (CurTime > 0) CurTime -= Time.deltaTime;

            if (CurTime < 0 && list.Count > 0)
            {
                collection.RemoveSubBuff();
                CurTime = Duration;
            }
        }

        public void ResetTime()
        {
            CurTime = Duration;
        }
    }
}