using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public abstract class ItemFactory<T> where T : Component
    {
        protected ObjectPool<T> pool; // 오브젝트 풀

        public ItemFactory(T[] objs) // 생성자, 인수로 오브젝트 생성할 프리팹 배열 입력
        {
            pool = new ObjectPool<T>(objs);
        }
        // 아이템 팩토리 메소드

        public abstract T CreateNew(int itemId); // 이름으로 오브젝트 생성

        public virtual T[] CreateNews(int itemId, int amount)
        {
            var returnValue = new T[amount];
            for (var i = 0; i < amount; i++) returnValue[i] = CreateNew(itemId);
            return returnValue;
        }

        public abstract T CreateRandom(); // 랜덤 오브젝트 생성
        public abstract List<T> CreateAll(); // 전부 생성

        public virtual void Return(T obj) // 오브젝트 반환
        {
            pool.Return(obj);
        }
    }
}