using System;
using UnityEngine;

namespace Default
{
    /// <summary>
    ///     로딩 단계에서 미리 올려둘 애셋 목록.
    ///     여기에 등록되지 않은 애셋은 게임플레이 도중 동기 로드되어 프레임 스파이크를 만든다.
    /// </summary>
    [CreateAssetMenu(fileName = "PreloadManifest", menuName = "Config/Preload Manifest")]
    public class PreloadManifest : ScriptableObject
    {
        [Header("Addressables 라벨 단위로 통째로 로드")]
        public string[] labels = Array.Empty<string>();

        [Header("개별 주소 지정")]
        public string[] addresses = Array.Empty<string>();

        [Header("풀에 미리 생성해 둘 오브젝트")]
        public PrewarmEntry[] prewarm = Array.Empty<PrewarmEntry>();

        [Serializable]
        public struct PrewarmEntry
        {
            public string address;
            [Min(1)] public int count;
        }
    }
}
