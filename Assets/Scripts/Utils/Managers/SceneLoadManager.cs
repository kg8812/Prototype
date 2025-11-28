using System.Text.RegularExpressions;
using Managers;
using UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Apis.Managers
{
    public enum SceneType
    {
        Init,
        Intro,
        Title,
        Loading,
        Other
    }

    public struct SceneData
    {
        public string sceneName;
        public SceneType sceneType;
        public bool isPlayerMustExist;
    }

    public class SceneLoadManager
    {
        public SceneData CurSceneData;

        // 지금 씬이 로딩중이냐?
        private bool isSceneLoading;
        public UnityEvent<SceneData> WhenSceneLoadBegin;
        public UnityEvent<SceneData> WhenSceneLoaded;

        public void Init()
        {
            isSceneLoading = false;

            WhenSceneLoadBegin = new UnityEvent<SceneData>();
            WhenSceneLoaded = new UnityEvent<SceneData>();

            SceneManager.activeSceneChanged += Fading;
            SceneManager.activeSceneChanged += OnSceneLoaded;
            WhenSceneLoaded.AddListener(_ => SceneLoaded());
            CurSceneData = GetNextSceneData("Init");
        }

        private void Fading(Scene scene1, Scene scene2)
        {
            FadeManager.instance.Fading(null, isFadeIn: false, isFadeOut: true);
        }

        public void SceneLoaded()
        {
            isSceneLoading = false;
        }

        public bool SceneLoad(string sceneName, bool isLoading = true)
        {
            if (isSceneLoading) return false;
            var pattern = @"^Sector(\d+)$";

            // 정규 표현식에 일치하는지 확인
            var match = Regex.Match(sceneName, pattern);
            if (match.Success)
            {
                // 일치하는 부분이 있다면 숫자를 추출
                var numberPart = match.Groups[1].Value;

                if (int.TryParse(numberPart, out var intValue))
                {
                    // GameManager.SectorMag.CurSector = intValue;
                }
            }

            isSceneLoading = true;

            if (isLoading)
            {
                FadeManager.instance.Fading(() =>
                {
                    WhenSceneLoadBegin.Invoke(CurSceneData);
                    CurSceneData = GetNextSceneData(sceneName);
                    if (GameManager.instance.Player != null)
                    {
                        if (!CurSceneData.isPlayerMustExist) GameManager.instance.DestroyPlayer();
                    }

                    LoadingSceneManager.LoadStage(sceneName);
                }, () => { }, isFadeIn: IsFading(CurSceneData), isFadeOut: false);
            }
            else
            {
                var nextData = GetNextSceneData(sceneName);
                FadeManager.instance.Fading(() =>
                {
                    WhenSceneLoadBegin.Invoke(CurSceneData);
                    CurSceneData = nextData;
                    if (GameManager.instance.Player != null)
                    {
                        if (!CurSceneData.isPlayerMustExist) GameManager.instance.DestroyPlayer();
                    }

                    SceneManager.LoadScene(sceneName);
                }, isFadeIn: IsFading(CurSceneData), isFadeOut: IsFading(nextData));
            }

            return true;
        }

        private bool IsFading(SceneData scene)
        {
            if (scene.sceneType == SceneType.Init || scene.sceneType == SceneType.Loading) return false;
            return true;
        }

        public SceneData GetNextSceneData(string sceneName)
        {
            SceneData newScene = new();
            newScene.sceneName = sceneName;

            if (sceneName == Define.SceneNames.Init)
            {
                newScene.sceneType = SceneType.Init;
                newScene.isPlayerMustExist = false;
            }
            else if (sceneName == Define.SceneNames.Loading)
            {
                newScene.sceneType = SceneType.Loading;
                newScene.isPlayerMustExist = false;
            }
            else if (sceneName == Define.SceneNames.Intro)
            {
                newScene.sceneType = SceneType.Intro;
                newScene.isPlayerMustExist = false;
            }
            else if (sceneName == Define.SceneNames.TitleSceneName)
            {
                newScene.sceneType = SceneType.Title;
                newScene.isPlayerMustExist = false;
            }
            else
            {
                newScene.sceneType = SceneType.Other;
                newScene.isPlayerMustExist = true;
            }

            return newScene;
        }

        private void OnSceneLoaded(Scene scene1, Scene scene2)
        {
            if (scene2.name == Define.SceneNames.Init) return;
            var newScene = GetNextSceneData(scene2.name);
            if (newScene.sceneType == SceneType.Loading) LoadingSceneManager.LoadLoadingScene();

            WhenSceneLoaded.Invoke(newScene);
        }
    }
}