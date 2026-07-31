using System.Collections.Generic;
using Default;
using UnityEngine;

namespace Apis
{
    /// <summary>
    ///     주소 단위 오브젝트 풀.
    ///     인스턴스는 순수 Unity 오브젝트이므로(ResourceUtil.Instantiate 참조) Addressables 핸들을 갖지 않는다.
    ///     대여된 오브젝트는 현재 씬으로 이동하므로 씬 전환 시 외부에서 파괴될 수 있으며,
    ///     이 클래스는 그런 오브젝트를 만나도 깨지지 않도록 방어한다.
    /// </summary>
    public class AddressablePooling
    {
        private static GameObject poolParent;

        // 대기 중인 오브젝트. 주소별로 나눠 보관한다.
        private readonly Dictionary<string, Queue<GameObject>> _idle = new();

        // 대기 큐에 들어있는 인스턴스 ID. 중복 반환을 O(1)로 검사하기 위한 것.
        private readonly HashSet<int> _idleIds = new();

        // 이 풀이 만든 모든 오브젝트의 추적 정보. 파괴된 오브젝트도 ID로 다룰 수 있도록 int를 키로 쓴다.
        private readonly Dictionary<int, ObjectInfo> _infos = new();

        private readonly string _name;
        private readonly GameObject pool;

        public AddressablePooling(string name)
        {
            _name = name;
            pool = new GameObject(name + " Pool");
            pool.transform.SetParent(PoolParent.transform);
        }

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

        /// <summary>
        ///     로딩 단계에서 미리 인스턴스를 만들어 둔다. 런타임 중 첫 생성 스파이크를 없애기 위한 것.
        /// </summary>
        public void Prewarm(string address, int count)
        {
            var idle = GetIdleQueue(address);

            for (var i = 0; i < count; i++)
            {
                var obj = CreateNew(address);
                if (obj == null) return; // 주소가 잘못된 경우이므로 반복해도 의미가 없다

                Park(obj, idle);
            }
        }

        public GameObject Get(string address, Vector2? pos = null)
        {
            var idle = GetIdleQueue(address);
            GameObject obj = null;

            // 씬 전환 등으로 외부에서 파괴된 오브젝트는 건너뛰고 추적 정보도 정리한다.
            while (idle.Count > 0)
            {
                var candidate = idle.Dequeue();
                var candidateId = candidate.GetInstanceID();
                _idleIds.Remove(candidateId);

                if (candidate != null)
                {
                    obj = candidate;
                    break;
                }

                _infos.Remove(candidateId);
            }

            if (obj == null)
            {
                obj = CreateNew(address);
                if (obj == null) return null;
            }

            obj.transform.SetParent(null);

            if (_infos.TryGetValue(obj.GetInstanceID(), out var info))
            {
                obj.transform.rotation = info.rotation;
                obj.transform.localScale = info.scale;
            }

            if (pos != null) obj.transform.position = (Vector2)pos;

            obj.SetActive(true);

            foreach (var poolObject in obj.GetComponents<IPoolObject>()) poolObject.OnGet();

            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null) return;

            var id = obj.GetInstanceID();

            // 이 풀이 만들지 않은 오브젝트는 그냥 파기한다.
            if (!_infos.TryGetValue(id, out var info))
            {
                Object.Destroy(obj);
                return;
            }

            if (!_idleIds.Add(id)) return; // 이미 반환된 오브젝트

            foreach (var poolObject in obj.GetComponents<IPoolObject>()) poolObject.OnReturn();

            obj.SetActive(false);
            obj.transform.SetParent(pool.transform);
            obj.transform.localPosition = info.position;
            obj.transform.rotation = info.rotation;
            obj.transform.localScale = info.scale;

            GetIdleQueue(info.address).Enqueue(obj);
        }

        /// <summary>
        ///     이 풀이 만든 오브젝트를 모두 파기하고 초기 상태로 되돌린다. 씬 전환 시 호출한다.
        /// </summary>
        public void Clear()
        {
            foreach (var info in _infos.Values)
                if (info.obj != null)
                    Object.Destroy(info.obj);

            _idle.Clear();
            _idleIds.Clear();
            _infos.Clear();
        }

        private GameObject CreateNew(string address)
        {
            // 부모를 지정하지 않고 생성해 프리팹 원본 트랜스폼 값을 그대로 캡처한다.
            var obj = ResourceUtil.Instantiate(address);
            if (obj == null)
            {
                Debug.LogError($"[Pool:{_name}] Failed to instantiate '{address}'.");
                return null;
            }

            _infos[obj.GetInstanceID()] = new ObjectInfo(obj, address);
            return obj;
        }

        private void Park(GameObject obj, Queue<GameObject> idle)
        {
            obj.SetActive(false);
            obj.transform.SetParent(pool.transform);
            idle.Enqueue(obj);
            _idleIds.Add(obj.GetInstanceID());
        }

        private Queue<GameObject> GetIdleQueue(string address)
        {
            if (_idle.TryGetValue(address, out var idle)) return idle;

            idle = new Queue<GameObject>();
            _idle.Add(address, idle);
            return idle;
        }

        private readonly struct ObjectInfo
        {
            public ObjectInfo(GameObject obj, string address)
            {
                this.obj = obj;
                var transform = obj.transform;
                rotation = transform.rotation;
                position = transform.localPosition;
                scale = transform.localScale;
                this.address = address;
            }

            public readonly GameObject obj;
            public readonly Quaternion rotation;
            public readonly Vector3 position;
            public readonly Vector3 scale;
            public readonly string address;
        }
    }
}
