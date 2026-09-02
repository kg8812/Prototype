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
        // [캐시] (주소,타입) → 로드된 애셋 본체. 조회 적중 시 여기서 바로 꺼내 준다.
        private readonly Dictionary<AssetKey, Object> _assets = new();

        // [소유권] (주소,타입) → 그 애셋을 붙잡고 있는 Addressables 핸들. Dispose가 해제할 대상 목록.
        // _assets와 항상 같은 키로 짝을 이룬다. 애셋은 "쓰려고", 핸들은 "놓아주려고" 들고 있다.
        private readonly Dictionary<AssetKey, AsyncOperationHandle> _assetHandles = new();

        // 주소만 알고 타입은 모르는 경우(라벨 프리로드)를 위한 보조 색인.
        // 프리로드가 채워두면 이후 Load<T>가 타입 키로 못 찾아도 여기서 찾아 재로드를 피한다.
        private readonly Dictionary<string, Object> _byAddress = new();

        // 라벨 조회 결과 캐시. 같은 라벨을 다시 요청했을 때 주소 전개를 반복하지 않기 위한 것이며,
        // 애셋의 소유권은 개별 주소 핸들(_assetHandles)이 갖는다.
        private readonly Dictionary<AssetKey, Array> _labelAssets = new();

        // 로그 식별용 이름("Global" / "Scene"). 동작에는 관여하지 않는다.
        private readonly string _name;

        public AssetScope(string name)
        {
            _name = name;
        }

        /// <summary>이 스코프가 붙잡고 있는 핸들 수. 진단용.</summary>
        public int LoadedCount => _assetHandles.Count;

        /// <summary>보유한 핸들을 전부 Addressables에 반납하고 캐시를 비운다. 이 스코프의 유일한 해제 경로.</summary>
        public void Dispose()
        {
            foreach (var handle in _assetHandles.Values)
                if (handle.IsValid())
                    Addressables.Release(handle);

            _assetHandles.Clear();
            _assets.Clear();
            _byAddress.Clear();
            _labelAssets.Clear();
        }

        #region 동기 조회

        /// <summary>
        ///     주소로 애셋 하나를 가져온다. 캐시에 없으면 그 자리에서 동기 로드한다(메인 스레드가 멈춘다).
        ///     T가 Component면 프리팹을 로드해 컴포넌트를 꺼내 준다.
        /// </summary>
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

        /// <summary>라벨이 붙은 애셋을 전부 동기 로드한다. 반환 배열은 캐시된 것이므로 호출부가 수정하면 안 된다.</summary>
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

        /// <summary>동기 로드 본체. 캐시 조회 → (없으면) 로드 후 완료까지 대기 → 등록. Load/LoadAsync가 공유하는 흐름의 동기 버전.</summary>
        /// <param name="report">라벨 전개로 불린 경우 false. 라벨 단위로 이미 한 번 보고했으므로 주소마다 또 경고하지 않는다.</param>
        private TAsset LoadAsset<TAsset>(string address, bool report = true) where TAsset : Object
        {
            if (TryGetCached<TAsset>(address, out var hit)) return hit;

            if (report) ReportSyncLoad(address, typeof(TAsset));

            var handle = Addressables.LoadAssetAsync<TAsset>(address);
            handle.WaitForCompletion();

            return Register<TAsset>(address, handle);
        }

        /// <summary>
        ///     라벨 동기 로드 본체. 라벨을 주소 목록으로 펼친 뒤 하나씩 로드한다.
        ///     Addressables.LoadAssetsAsync는 핸들 하나로 배열 전체를 붙잡지만 어느 애셋이 어느 주소인지
        ///     남기지 않아서, 이후 Load(주소) 호출이 캐시(_byAddress)에 적중하지 못한다.
        ///     비동기 프리로드(PreloadLabelsAsync)가 이미 쓰는 방식에 맞춘다.
        /// </summary>
        private TAsset[] LoadAssets<TAsset>(string label) where TAsset : Object
        {
            var key = new AssetKey(label, typeof(TAsset));

            if (_labelAssets.TryGetValue(key, out var cached)) return (TAsset[])cached;

            ReportSyncLoad($"[label] {label}", typeof(TAsset));

            var addresses = ResolveAddresses(new[] { label });
            if (addresses.Count == 0)
            {
                Debug.LogError($"[AssetScope:{_name}] Label '{label}' resolved to no addresses.");
                return null;
            }

            var loaded = new List<TAsset>(addresses.Count);
            foreach (var address in addresses)
            {
                var asset = LoadAsset<TAsset>(address, false);
                if (asset != null) loaded.Add(asset);
            }

            var result = loaded.ToArray();
            _labelAssets[key] = result;
            return result;
        }

        #endregion

        #region 비동기 로드 / 프리로드

        /// <summary>Addressables 시스템 자체를 초기화한다(카탈로그 로드). 어떤 로드보다 먼저, 게임당 한 번.</summary>
        public static async Awaitable InitializeAsync(CancellationToken ct = default)
        {
            var handle = Addressables.InitializeAsync(false);
            
            if (!await WaitFor(handle, ct))
                Debug.LogError("[AssetScope] Addressables initialization failed.");

            if (handle.IsValid()) Addressables.Release(handle);
        }

        /// <summary>Load의 비동기 버전. 프레임을 넘기며 기다리므로 게임이 멈추지 않는다. 결과는 동일하게 캐시된다.</summary>
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

        /// <summary>주소 목록을 하나씩 비동기 로드해 이 스코프에 미리 올려둔다. 진행률은 개수 기준.</summary>
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

        /// <summary>ResolveAddressesAsync의 동기 버전. 동기 라벨 로드 경로가 쓴다.</summary>
        private List<string> ResolveAddresses(IReadOnlyList<string> labels)
        {
            var addresses = new List<string>();
            if (labels == null || labels.Count == 0) return addresses;

            var valid = labels.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            if (valid.Length == 0) return addresses;

            var handle = Addressables.LoadResourceLocationsAsync(valid, Addressables.MergeMode.Union);
            handle.WaitForCompletion();

            if (handle.Status == AsyncOperationStatus.Succeeded && handle.Result != null)
            {
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

        /// <summary>라벨 목록 → 그 라벨이 가리키는 주소 목록으로 펼친다. 애셋을 로드하지는 않는다(위치 조회만).</summary>
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

        /// <summary>LoadAsset의 비동기 버전. 캐시 조회 → 로드 → 등록의 흐름은 같고 대기 방식만 다르다.</summary>
        private async Awaitable<TAsset> LoadAssetAsync<TAsset>(string address, CancellationToken ct) where TAsset : Object
        {
            if (TryGetCached<TAsset>(address, out var hit)) return hit;

            var handle = Addressables.LoadAssetAsync<TAsset>(address);
            await WaitFor(handle, ct);

            return Register<TAsset>(address, handle);
        }

        /// <summary>핸들이 끝날 때까지 프레임 단위로 대기하고 성공 여부를 돌려준다.</summary>
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

        /// <summary>동기 로드가 발생했음을 주소당 한 번만 기록/경고한다.</summary>
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

        /// <summary>
        ///     캐시에서만 찾는다. 타입 키(_assets) → 주소 색인(_byAddress) 순으로 본다.
        ///     캐시에 있으나 실제 오브젝트가 죽었으면 그 항목을 걷어내고 실패 처리한다.
        /// </summary>
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

        /// <summary>
        ///     로드 결과를 이 스코프의 소유로 등록한다(핸들 + 애셋 + 주소 색인 3곳).
        ///     실패했거나 이미 등록된 경우엔 새 핸들을 즉시 반납한다.
        /// </summary>
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

        /// <summary>특정 항목 하나만 해제하고 세 자료구조에서 지운다. Dispose의 단건 버전.</summary>
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
