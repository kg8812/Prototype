using System;
using System.Collections.Generic;
using UnityEngine;

namespace Default
{
    /// <summary>
    ///     씬 이름 → 그 씬에서만 쓰는 애셋 목록.
    ///     로딩 화면이 씬 이름만 알고 있으므로, 매니페스트를 찾으려면 이 표가 필요하다.
    /// </summary>
    [CreateAssetMenu(fileName = "SceneManifestTable", menuName = "Config/Scene Manifest Table")]
    public class SceneManifestTable : ScriptableObject
    {
        [SerializeField] private Entry[] entries = Array.Empty<Entry>();

        private Dictionary<string, PreloadManifest> _lookup;

        public PreloadManifest Find(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName)) return null;

            if (_lookup == null)
            {
                _lookup = new Dictionary<string, PreloadManifest>();
                foreach (var entry in entries)
                {
                    if (string.IsNullOrEmpty(entry.sceneName)) continue;
                    if (!_lookup.TryAdd(entry.sceneName, entry.manifest))
                        Debug.LogWarning($"[SceneManifestTable] Duplicated scene entry: {entry.sceneName}");
                }
            }

            return _lookup.GetValueOrDefault(sceneName);
        }

        [Serializable]
        private struct Entry
        {
            public string sceneName;
            public PreloadManifest manifest;
        }
    }
}
