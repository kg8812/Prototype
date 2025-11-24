using Default;
using System.Collections.Generic;
using UnityEngine;

namespace Apis
{
    public class AddressablePooling
    {
        struct ObjectInfo
        {
            public ObjectInfo(Transform transform,string address)
            {
                rotation = transform.rotation;
                position = transform.localPosition;
                scale = transform.localScale;
                this.address = address;
            }
            public readonly Quaternion rotation;
            public readonly Vector3 position;
            public readonly Vector3 scale;
            public readonly string address;
        }
        readonly GameObject pool;

        readonly Dictionary<string, Queue<GameObject>> queue = new();
        private readonly Dictionary<GameObject, ObjectInfo> _objectInfos = new();
        static GameObject poolParent;

        public static GameObject PoolParent
        {
            get
            {
                if (poolParent == null)
                {
                    poolParent = new GameObject("Pooling");
                    Object.DontDestroyOnLoad(poolParent.gameObject);
                }

                return poolParent;
            }
        }
        public AddressablePooling(string name)
        {          
            pool = new GameObject(name + " Pool");
            pool.transform.SetParent(PoolParent.transform);
        }

        void EnQueue(GameObject obj,string address)
        {
            if (obj != null)
            {
                if (!queue.ContainsKey(address))
                {
                    queue.Add(address, new Queue<GameObject>());
                }

                obj.SetActive(false);
                queue[address].Enqueue(obj);
                if (_objectInfos.ContainsKey(obj)) return;
                ObjectInfo info = new(obj.transform,address);
                _objectInfos.Add(obj,info);
            }
        }
        void CreateNew(string address)
        {   
            GameObject obj = ResourceUtil.Instantiate(address, pool.transform);
           
            EnQueue(obj,address);
        }

        public GameObject Get(string address, Vector2? pos = null)
        {
            GameObject obj = null;

            if (!queue.ContainsKey(address))
            {
                queue.Add(address, new Queue<GameObject>());
            }

            if (queue[address].Count == 0)
            {
                CreateNew(address);
            }
            
            if (queue[address].Count > 0)
            {
                obj = queue[address].Dequeue();
                obj.transform.SetParent(null);

                if (pos != null)
                {
                    obj.transform.position = (Vector2)pos;
                }

                obj.SetActive(true);
                if (_objectInfos.TryGetValue(obj, out var info))
                {
                    obj.transform.rotation = info.rotation;
                    obj.transform.localScale = info.scale;
                }
            }

            if (obj != null)
            {
                var list = obj.GetComponents<IPoolObject>();
                foreach (var temp in list)
                {
                    temp.OnGet();
                }
            }
            
            return obj;
        }       

        public void Return(GameObject obj)
        {
            if (obj == null) return;

            if (_objectInfos.TryGetValue(obj, out var info))
            {
                if (obj != null)
                {
                    var list = obj.GetComponents<IPoolObject>();
                    foreach (var temp in list)
                    {
                        temp.OnReturn();
                    }
                }
                string address = info.address;
               
                if (queue.ContainsKey(address))
                {
                    if (queue[address].Contains(obj)) return;
                    queue[address].Enqueue(obj);
                    obj.transform.SetParent(pool.transform);
                    obj.SetActive(false);
                    Utils.ActionAfterFrame(() =>
                    {
                        if (obj.activeSelf) return;
                        
                        obj.transform.localPosition = info.position;
                        obj.transform.rotation = info.rotation;
                        obj.transform.localScale = info.scale;
                    });
                }
            }
            else
            {
                Object.Destroy(obj);
            }
            
        }
    }
}