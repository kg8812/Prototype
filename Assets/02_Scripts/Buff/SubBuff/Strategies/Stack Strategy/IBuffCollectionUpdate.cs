using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public interface IBuffCollectionUpdate : IObserver<List<SubBuff>>
    {
        public void Update();
        public void ResetTime();
        public float Duration { get; set; }
        public float CurTime { get; set; }
    }

    public class SingleStackDecrease : IBuffCollectionUpdate
    {
        List<SubBuff> list;
        public float Duration { get; set; }

        public float CurTime { get; set; }
        SubBuffCollection collection;

        public SingleStackDecrease(SubBuffCollection collection,float duration)
        {
            this.collection = collection;
            list = collection.List;
            this.Duration = duration;
            CurTime = duration;
        }

        public void Notify(List<SubBuff> value)
        {
            list = value;
        }

        public void Update()
        {
            if(CurTime > 0)
            {
                CurTime -= Time.deltaTime;
            }

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