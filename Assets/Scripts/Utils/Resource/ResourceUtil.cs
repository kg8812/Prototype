using System;
using System.Threading;
using Apis;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Default
{
    /// <summary>
    ///     애셋 로드 진입점.
    ///     핸들의 소유와 해제는 전적으로 AssetScope가 담당하므로,
    ///     호출부에서 Addressables.Release를 직접 호출해서는 안 된다.
    /// </summary>
    public static class ResourceUtil
    {
        private static SceneManifestTable _sceneManifests;

        /// <summary>
        ///     이미 올라와 있는 스코프가 있으면 그걸 쓰고, 없을 때만 lifetime이 가리키는 스코프로 로드한다.
        ///     씬 프리로드가 Scene 스코프에 올려둔 애셋을, 호출부가 lifetime을 넘기지 않아도 찾아 쓸 수 있게 하기 위한 것.
        /// </summary>
        public static T Load<T>(string path, AssetLifetime lifetime = AssetLifetime.Global) where T : Object
        {
            if (AssetRegistry.Scene.TryGet<T>(path, out var inScene)) return inScene;
            if (AssetRegistry.Global.TryGet<T>(path, out var inGlobal)) return inGlobal;

            return AssetRegistry.Of(lifetime).Load<T>(path);
        }

        public static T[] LoadAll<T>(string label, AssetLifetime lifetime = AssetLifetime.Global) where T : Object
        {
            return AssetRegistry.Of(lifetime).LoadAll<T>(label);
        }

        public static Awaitable<T> LoadAsync<T>(string path, CancellationToken ct = default,
            AssetLifetime lifetime = AssetLifetime.Global) where T : Object
        {
            return AssetRegistry.Of(lifetime).LoadAsync<T>(path, ct);
        }

        /// <summary>
        ///     프리팹은 스코프가 핸들 하나로 보유하고, 인스턴스는 순수 Unity 오브젝트로 생성한다.
        ///     인스턴스마다 Addressables 핸들이 생기지 않으므로 풀링과 충돌하지 않으며,
        ///     인스턴스 파기는 Object.Destroy만으로 충분하다.
        /// </summary>
        public static GameObject Instantiate(string path, Transform parent = null,
            AssetLifetime lifetime = AssetLifetime.Global)
        {
            var prefab = Load<GameObject>(path, lifetime);
            if (prefab == null) return null;

            var go = Object.Instantiate(prefab);
            if (parent != null) go.transform.SetParent(parent);

            var index = go.name.IndexOf("(Clone)", StringComparison.Ordinal);
            if (index > 0)
                go.name = go.name[..index];

            return go;
        }

        #region 프리로드 / 정리

        public static Awaitable InitializeAsync(CancellationToken ct = default)
        {
            return AssetScope.InitializeAsync(ct);
        }

        /// <summary>
        ///     매니페스트에 등록된 애셋을 모두 올리고, 풀도 함께 채운다.
        /// </summary>
        public static async Awaitable PreloadAsync(PreloadManifest manifest, IProgress<float> progress = null,
            AssetLifetime lifetime = AssetLifetime.Global, CancellationToken ct = default)
        {
            if (manifest == null)
            {
                progress?.Report(1f);
                return;
            }

            var scope = AssetRegistry.Of(lifetime);

            await scope.PreloadLabelsAsync(manifest.labels, null, ct);
            await scope.PreloadAddressesAsync(manifest.addresses, null, ct);

            foreach (var entry in manifest.prewarm)
            {
                if (string.IsNullOrEmpty(entry.address)) continue;

                // 프리팹을 먼저 대상 스코프에 올려야 Prewarm이 Global로 새지 않는다.
                await scope.LoadAsync<GameObject>(entry.address, ct);
                IObjectFactory.PrewarmPool(entry.address, Mathf.Max(1, entry.count));
                await Awaitable.NextFrameAsync(ct);
            }

            progress?.Report(1f);
        }

        /// <summary>씬 전용 애셋을 Scene 스코프로 올린다. 로딩 화면이 씬을 활성화하기 직전에 호출한다.</summary>
        public static Awaitable PreloadSceneAsync(string sceneName, IProgress<float> progress = null,
            CancellationToken ct = default)
        {
            var manifest = _sceneManifests != null ? _sceneManifests.Find(sceneName) : null;

            if (manifest == null && _sceneManifests != null)
                Debug.Log($"[ResourceUtil] No preload manifest registered for scene '{sceneName}'.");

            return PreloadAsync(manifest, progress, AssetLifetime.Scene, ct);
        }

        /// <summary>씬별 매니페스트 표를 등록한다. Initializer가 시작 시 한 번 호출한다.</summary>
        public static void RegisterSceneManifests(SceneManifestTable table)
        {
            _sceneManifests = table;
        }

        /// <summary>프리로드되지 않아 동기 로드된 주소 목록을 출력한다. 매니페스트 작성용.</summary>
        public static void LogPreloadReport()
        {
            AssetScope.LogSyncLoadReport();
        }

        /// <summary>
        ///     씬 전용 애셋과 게임플레이 오브젝트 풀을 정리한다. 씬 전환 직전에 호출한다.
        /// </summary>
        public static void ReleaseSceneAssets()
        {
            IObjectFactory.ClearPool();
            AssetRegistry.ReleaseScene();
        }

        #endregion
    }
}
