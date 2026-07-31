using System;
using System.Threading;
using Default;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Apis
{
    public enum LoadingSceneType
    {
        Tip,
        NextStage
    }

    public static class LoadingSceneManager
    {
        // 진행바가 목표치를 따라가는 속도(초당). 값이 클수록 즉각적으로 반응한다.
        private const float FillSpeed = 1.5f;

        // 전체 진행률 중 씬 로드가 차지하는 비중. 나머지는 애셋 프리로드 몫이다.
        private const float SceneLoadWeight = 0.5f;

        private static string _nextScene;

        /// <summary>로딩 UI가 읽어가는 0~1 진행률.</summary>
        public static float Progress { get; private set; }

        public static void LoadLoadingScene()
        {
            RunLoad();
        }

        public static void LoadStage(string sceneName)
        {
            _nextScene = sceneName;
            SceneManager.LoadScene("LoadingScene");
        }

        private static async void RunLoad()
        {
            try
            {
                await LoadRoutine(Application.exitCancellationToken);
            }
            catch (OperationCanceledException)
            {
                // 종료 중이면 조용히 빠져나간다.
            }
        }

        private static async Awaitable LoadRoutine(CancellationToken ct)
        {
            Progress = 0f;
            await Awaitable.NextFrameAsync(ct);

            var op = SceneManager.LoadSceneAsync(_nextScene);
            op.allowSceneActivation = false;

            // 1) 씬 로드. 활성화를 막아두면 Unity의 progress는 0.9에서 멈춘다.
            while (op.progress < 0.9f)
            {
                Advance(op.progress / 0.9f * SceneLoadWeight);
                await Awaitable.NextFrameAsync(ct);
            }

            // 2) 씬이 시작되기 전에 그 씬 전용 애셋을 올린다.
            //    여기서 올려두지 않으면 씬 진입 직후 동기 로드가 발생한다.
            var target = SceneLoadWeight;
            var preloadProgress = new Progress<float>(v => target = SceneLoadWeight + v * (1f - SceneLoadWeight));

            var preload = ResourceUtil.PreloadSceneAsync(_nextScene, preloadProgress, ct);

            while (!preload.IsCompleted)
            {
                Advance(target);
                await Awaitable.NextFrameAsync(ct);
            }

            // 폴링만 하면 프리로드 중 발생한 예외가 묻히므로 반드시 await로 결과를 관측한다.
            await preload;

            // 3) 진행바가 끝까지 찬 뒤에 씬을 띄운다.
            while (Progress < 1f)
            {
                Advance(1f);
                await Awaitable.NextFrameAsync(ct);
            }

            op.allowSceneActivation = true;
        }

        private static void Advance(float target)
        {
            Progress = Mathf.MoveTowards(Progress, target, Time.unscaledDeltaTime * FillSpeed);
        }
    }
}
