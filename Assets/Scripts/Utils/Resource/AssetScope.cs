using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Default
{
    /// <summary>
    ///     수명이 같은 애셋들을 하나로 묶어 관리하는 단위.
    ///     로드한 AsyncOperationHandle의 소유권은 전적으로 이 클래스에 있으며, 해제는 Dispose로만 이루어진다.
    ///     따라서 외부에서 Addressables.Release를 직접 호출해서는 안 된다.
    /// </summary>
    public sealed class AssetScope : IDisposable
    {
        private readonly Dictionary<AssetKey, Object> _assets = new();
        private readonly Dictionary<AssetKey, AsyncOperationHandle> _assetHandles = new();

        // 주소만 알고 타입은 모르는 경우(라벨 프리로드)를 위한 보조 색인.
        // 프리로드가 채워두면 이후 Load<T>가 타입 키로 못 찾아도 여기서 찾아 재로드를 피한다.
        private readonly Dictionary<string, Object> _byAddress = new();

        private readonly Dictionary<AssetKey, Array> _labelAssets = new();
        private readonly Dictionary<AssetKey, AsyncOperationHandle> _labelHandles = new();

        private readonly string _name;

        public AssetScope(string name)
        {
            _name = name;
        }

        public int LoadedCount => _assetHandles.Count + _labelHandles.Count;

        public void Dispose()
        {
            foreach (var handle in _assetHandles.Values)
                if (handle.IsValid())
                    Addressables.Release(handle);

            foreach (var handle in _labelHandles.Values)
                if (handle.IsValid())
                    Addressables.Release(handle);

            _assetHandles.Clear();
            _assets.Clear();
            _byAddress.Clear();
            _labelHandles.Clear();
            _labelAssets.Clear();
        }

        #region 동기 조회

        public T Load<T>(string address) where T : Object
        {
            if (string.IsNullOrEmpty(address))
            {
                Debug.LogError($"[AssetScope:{_name}] address is null or empty.");
                return null;
            }

            // 컴포넌트는 프리팹(GameObject)으로 로드한 뒤 꺼낸다.
            // 핸들이 항상 GameObject 기준으로 보관되므로 해제 경로가 유효하게 유지된다.
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var prefab = LoadAsset<GameObject>(address);
                if (prefab == null) return null;

                var component = prefab.GetComponent<T>();
                if (component == null)
                    Debug.LogError($"[AssetScope:{_name}] '{address}' has no component {typeof(T).Name}.");

                return component;
            }

            return LoadAsset<T>(address);
        }

        /// <summary>
        ///     이미 이 스코프에 올라와 있을 때만 반환한다. 없으면 로드하지 않고 false.
        ///     여러 스코프를 순서대로 뒤질 때 쓴다.
        /// </summary>
        public bool TryGet<T>(string address, out T result) where T : Object
        {
            result = null;
            if (string.IsNullOrEmpty(address)) return false;

            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                if (!TryGetCached<GameObject>(address, out var prefab) || prefab == null) return false;

                result = prefab.GetComponent<T>();
                return result != null;
            }

            return TryGetCached(address, out result) && result != null;
        }

        public T[] LoadAll<T>(string label) where T : Object
        {
            if (string.IsNullOrEmpty(label))
            {
                Debug.LogError($"[AssetScope:{_name}] label is null or empty.");
                return Array.Empty<T>();
            }

            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var prefabs = LoadAssets<GameObject>(label);
                return prefabs == null
                    ? Array.Empty<T>()
                    : prefabs.Select(x => x.GetComponent<T>()).Where(x => x != null).ToArray();
            }

            return LoadAssets<T>(label) ?? Array.Empty<T>();
        }

        private TAsset LoadAsset<TAsset>(string address) where TAsset : Object
        {
            if (TryGetCached<TAsset>(address, out var hit)) return hit;

            ReportSyncLoad(address, typeof(TAsset));

            var handle = Addressables.LoadAssetAsync<TAsset>(address);
            handle.WaitForCompletion();

            return Register<TAsset>(address, handle);
        }

        private TAsset[] LoadAssets<TAsset>(string label) where TAsset : Object
        {
            var key = new AssetKey(label, typeof(TAsset));

            if (_labelAssets.TryGetValue(key, out var cached)) return (TAsset[])cached;

            ReportSyncLoad($"[label] {label}", typeof(TAsset));

            var handle = Addressables.LoadAssetsAsync<TAsset>(label, null);
            handle.WaitForCompletion();

            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            {
                Debug.LogError($"[AssetScope:{_name}] Failed to load label '{label}' as {typeof(TAsset).Name}.");
                if (handle.IsValid()) Addressables.Release(handle);
                return null;
            }

            var result = handle.Result.ToArray();
            _labelHandles[key] = handle;
            _labelAssets[key] = result;
            return result;
        }

        #endregion

        #region 비동기 로드 / 프리로드

        public static async Awaitable InitializeAsync(CancellationToken ct = default)
        {
            var handle = Addressables.InitializeAsync(false);
            
            if (!await WaitFor(handle, ct))
                Debug.LogError("[AssetScope] Addressables initialization failed.");

            if (handle.IsValid()) Addressables.Release(handle);
        }

        public async Awaitable<T> LoadAsync<T>(string address, CancellationToken ct = default) where T : Object
        {
            if (string.IsNullOrEmpty(address))
            {
                Debug.LogError($"[AssetScope:{_name}] address is null or empty.");
                return null;
            }

            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var prefab = await LoadAssetAsync<GameObject>(address, ct);
                return prefab == null ? null : prefab.GetComponent<T>();
            }

            return await LoadAssetAsync<T>(address, ct);
        }

        /// <summary>
        ///     라벨에 속한 애셋을 모두 이 스코프로 끌어온다.
        ///     라벨을 주소 목록으로 먼저 펼친 뒤 개별 로드하므로, 이후 Load(주소) 호출이 캐시에 그대로 적중한다.
        /// </summary>
        public async Awaitable PreloadLabelsAsync(IReadOnlyList<string> labels, IProgress<float> progress = null,
            CancellationToken ct = default)
        {
            
            var addresses = await ResolveAddressesAsync(labels, ct);
            await PreloadAddressesAsync(addresses, progress, ct);
        }

        public async Awaitable PreloadAddressesAsync(IReadOnlyList<string> addresses, IProgress<float> progress = null,
            CancellationToken ct = default)
        {
            if (addresses == null || addresses.Count == 0)
            {
                progress?.Report(1f);
                return;
            }

            for (var i = 0; i < addresses.Count; i++)
            {
                await LoadAssetAsync<Object>(addresses[i], ct);
                progress?.Report((i + 1) / (float)addresses.Count);
            }
        }

        private async Awaitable<List<string>> ResolveAddressesAsync(IReadOnlyList<string> labels, CancellationToken ct)
        {
            var addresses = new List<string>();
            if (labels == null || labels.Count == 0) return addresses;

            var valid = labels.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (valid.Length == 0) return addresses;

            var handle = Addressables.LoadResourceLocationsAsync(valid, Addressables.MergeMode.Union);

            if (await WaitFor(handle, ct))
            {
                // 같은 애셋이 여러 라벨에 걸려 있을 수 있으므로 주소 기준으로 중복을 제거한다.
                var seen = new HashSet<string>();
                foreach (var location in handle.Result)
                    if (seen.Add(location.PrimaryKey))
                        addresses.Add(location.PrimaryKey);
            }
            else
            {
                Debug.LogError($"[AssetScope:{_name}] Failed to resolve labels: {string.Join(", ", valid)}");
            }

            // 위치 목록 핸들은 애셋을 붙잡지 않으므로 즉시 해제해도 된다.
            if (handle.IsValid()) Addressables.Release(handle);

            return addresses;
        }

        private async Awaitable<TAsset> LoadAssetAsync<TAsset>(string address, CancellationToken ct) where TAsset : Object
        {
            if (TryGetCached<TAsset>(address, out var hit)) return hit;

            var handle = Addressables.LoadAssetAsync<TAsset>(address);
            await WaitFor(handle, ct);

            return Register<TAsset>(address, handle);
        }

        private static async Awaitable<bool> WaitFor(AsyncOperationHandle handle, CancellationToken ct)
        {
            while (!handle.IsDone) await Awaitable.NextFrameAsync(ct);

            return handle.Status == AsyncOperationStatus.Succeeded;
        }

        #endregion

        #region 프리로드 누락 진단

        // 프리로드되지 않아 동기 로드로 떨어진 주소들.
        // WaitForCompletion은 메인 스레드를 막고 WebGL/원격 번들에서는 동작하지 않으므로,
        // 여기 모이는 주소는 결국 PreloadManifest로 옮겨야 한다.
        private static readonly HashSet<string> SyncLoaded = new();

        /// <summary>동기 로드 경고 출력 여부. 목록이 너무 시끄러우면 끌 수 있다.</summary>
        public static bool WarnOnSyncLoad = true;

        public static IReadOnlyCollection<string> SyncLoadedAddresses => SyncLoaded;

        private void ReportSyncLoad(string address, Type type)
        {
            if (!SyncLoaded.Add($"{address} ({type.Name})")) return;

            if (WarnOnSyncLoad)
                Debug.LogWarning(
                    $"[AssetScope:{_name}] 프리로드 누락 → 동기 로드: '{address}' as {type.Name}");
        }

        /// <summary>지금까지 동기 로드된 주소를 한 번에 출력한다. 매니페스트 작성용.</summary>
        public static void LogSyncLoadReport()
        {
            if (SyncLoaded.Count == 0)
            {
                Debug.Log("[AssetScope] 동기 로드 없음. 프리로드가 완전합니다.");
                return;
            }

            Debug.LogWarning(
                $"[AssetScope] 프리로드 누락 {SyncLoaded.Count}건 — PreloadManifest에 추가하세요:\n"
                + string.Join("\n", SyncLoaded));
        }

        public static void ClearSyncLoadReport()
        {
            SyncLoaded.Clear();
        }

        // 도메인 리로드를 끈 경우 이전 플레이 세션의 목록이 남지 않도록 초기화한다.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetDiagnostics()
        {
            SyncLoaded.Clear();
        }

        #endregion

        #region 캐시 조회 / 등록

        private bool TryGetCached<TAsset>(string address, out TAsset result) where TAsset : Object
        {
            var key = new AssetKey(address, typeof(TAsset));

            if (_assets.TryGetValue(key, out var cached))
            {
                if (cached != null)
                {
                    result = (TAsset)cached;
                    return true;
                }

                // 스코프 밖에서 Addressables.Release가 호출되어 애셋이 이미 파기된 경우.
                Debug.LogWarning($"[AssetScope:{_name}] '{address}' was released outside the scope. Reloading.");
                Purge(key, address);
            }

            // 프리로드가 다른 타입 키로 이미 올려둔 경우.
            if (_byAddress.TryGetValue(address, out var preloaded) && preloaded is TAsset typed)
            {
                result = typed;
                return true;
            }

            result = null;
            return false;
        }

        private TAsset Register<TAsset>(string address, AsyncOperationHandle<TAsset> handle) where TAsset : Object
        {
            if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
            {
                Debug.LogError($"[AssetScope:{_name}] Failed to load '{address}' as {typeof(TAsset).Name}.");
                if (handle.IsValid()) Addressables.Release(handle); // 실패한 핸들도 반드시 해제
                return null;
            }

            var key = new AssetKey(address, typeof(TAsset));

            // 경합 등으로 이미 등록되어 있으면 새 핸들은 버린다. 주소당 핸들은 하나만 유지한다.
            if (_assetHandles.ContainsKey(key))
            {
                Addressables.Release(handle);
                return (TAsset)_assets[key];
            }

            _assetHandles[key] = handle;
            _assets[key] = handle.Result;
            _byAddress[address] = handle.Result;
            return handle.Result;
        }

        private void Purge(AssetKey key, string address)
        {
            if (_assetHandles.TryGetValue(key, out var handle) && handle.IsValid())
                Addressables.Release(handle);

            _assetHandles.Remove(key);
            _assets.Remove(key);
            _byAddress.Remove(address);
        }

        #endregion

        // 같은 주소를 서로 다른 타입으로 로드할 수 있으므로 (주소, 타입)을 키로 쓴다.
        private readonly struct AssetKey : IEquatable<AssetKey>
        {
            private readonly string _id;
            private readonly Type _type;

            public AssetKey(string id, Type type)
            {
                _id = id;
                _type = type;
            }

            public bool Equals(AssetKey other)
            {
                return _type == other._type && _id == other._id;
            }

            public override bool Equals(object obj)
            {
                return obj is AssetKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(_id, _type);
            }

            public override string ToString()
            {
                return $"{_id}({_type.Name})";
            }
        }
    }
}
