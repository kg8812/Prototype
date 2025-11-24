using Apis;
using System.Collections.Generic;
using Default;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    // 오브젝트 풀링
    // 제네릭에 생성할 타입 입력 후 사용
    
    readonly GameObject pool; // 생성된 오브젝트들 정리용, 정리 외 용도 X

    // 생성용 프리팹 Dictionary, Key 값으론 프리팹 이름 들어감
    readonly IDictionary<string, T> prefabs = new Dictionary<string, T>();

    // 오브젝트들 관리용 Queue Dictionary, Key 값으로 동일하게 프리팹 이름 들어감
    readonly IDictionary<string, Queue<T>> queue = new Dictionary<string, Queue<T>>();

    
    // 생성자, 생성할 프리팹 배열 입력
    
    public ObjectPool(T[] objs)
    {
        // 클래스 이름으로 pool 오브젝트 이름 설정
        pool = new GameObject(typeof(T).FullName + " Pool");
        pool.transform.SetParent(AddressablePooling.PoolParent.transform);

        // 프리팹 개수만큼 Dictionary에 요소 추가
        for (int i = 0; i < objs.Length; i++)
        {
            prefabs.TryAdd(objs[i].name, objs[i]);
            queue.TryAdd(objs[i].name, new Queue<T>());
        }
    }

    void CreateNew(string name) // 내부용 오브젝트 생성 함수, 프리팹 이름 매개변수로 기입
    {
        // Queue에 남은 오브젝트가 없어서 새로 생성해야할 때 호출함

        // 프리팹 목록에 포함되어 있을 경우 새로 생성하여 반환
        if (prefabs.ContainsKey(name))
        {           
            T obj = Object.Instantiate(prefabs[name]).GetOrAddComponent<T>();
            obj.name = name;
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(pool.transform);
            queue[name].Enqueue(obj);           
        }      
    }
    public T Get(string name) // 외부에서 오브젝트 가져올 때 사용, 매개변수로 프리팹 이름 기입
    {
        T obj;

        //큐 목록에 포함되어 있을 경우 아이템을 반환함
        if (queue.ContainsKey(name))
        {
            // 오브젝트가 남아있을 시 Dequeue로 가져옴
            if (queue[name].Count == 0)
            { 
                CreateNew(name);                
            }

            obj = queue[name].Dequeue();
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);

            return obj;
        }

        return null;
    }
    
    // 외부에서 호출하는 오브젝트 반환용 함수
    // 오브젝트 풀링을 사용할 시, Destroy가 아닌 이 함수를 이용 해야함
    public void Return(T obj)
    {
        // Queue에 포함된 오브젝트일시 queue에 추가, 아닐시 그냥 Destroy

        if (queue.ContainsKey(obj.name))
        {
            queue[obj.name].Enqueue(obj);
            obj.gameObject.SetActive(false);
            obj.transform.SetParent(pool.transform);
        }
        else
        {
            Object.Destroy(obj.gameObject);
        }
    }
}

