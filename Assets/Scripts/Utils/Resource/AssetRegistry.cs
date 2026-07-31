using UnityEngine;

namespace Default
{
    /// <summary>
    ///     애셋의 수명 단위. 개별 애셋을 추적하는 대신 수명이 같은 것끼리 묶어 일괄 해제한다.
    /// </summary>
    public enum AssetLifetime
    {
        /// <summary>게임 종료까지 유지. 공용 UI 프리팹, 사운드 믹서, 공용 데이터 등.</summary>
        Global,

        /// <summary>씬을 벗어날 때 일괄 해제. 스테이지 전용 몬스터/배경/BGM 등.</summary>
        Scene
    }

    /// <summary>
    ///     AssetScope 보관소. 애셋 해제는 이곳을 통해서만 이루어진다.
    /// </summary>
    public static class AssetRegistry
    {
        public static AssetScope Global { get; private set; } = new("Global");
        public static AssetScope Scene { get; private set; } = new("Scene");

        public static AssetScope Of(AssetLifetime lifetime)
        {
            return lifetime == AssetLifetime.Scene ? Scene : Global;
        }

        /// <summary>씬 전환 시 호출. 해당 씬이 로드한 애셋을 일괄 해제한다.</summary>
        public static void ReleaseScene()
        {
            Scene.Dispose();
            Scene = new AssetScope("Scene");
        }

        public static void ReleaseAll()
        {
            Scene.Dispose();
            Global.Dispose();
            Scene = new AssetScope("Scene");
            Global = new AssetScope("Global");
        }

        // Enter Play Mode Options로 도메인 리로드를 끈 경우 static이 유지되어
        // 이전 플레이 세션의 죽은 핸들이 남는다. 플레이 진입 시 초기화한다.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Global = new AssetScope("Global");
            Scene = new AssetScope("Scene");
        }
    }
}
