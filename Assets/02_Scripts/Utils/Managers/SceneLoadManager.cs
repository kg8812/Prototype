using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UI;
using Managers;
using Save.Schema;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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
        public UnityEvent<SceneData> WhenSceneLoadBegin;
        public UnityEvent<SceneData> WhenSceneLoaded;


        public SceneData CurSceneData;
        
        // 지금 씬이 로딩중이냐?
        private bool isSceneLoading = false;

        public void Init()
        {
            this.isSceneLoading = false;

            WhenSceneLoadBegin = new();
            WhenSceneLoaded = new();

            SceneManager.activeSceneChanged += Fading;
            SceneManager.activeSceneChanged += OnSceneLoaded;
            SceneManager.activeSceneChanged += GameManager.SectorMag.UnLoadAllSectors;
            WhenSceneLoaded.AddListener(_ => SceneLoaded());
            CurSceneData = GetNextSceneData("Init");
        }

        void Fading(Scene scene1,Scene scene2)
        {
            FadeManager.instance.Fading(null,isFadeIn:false,isFadeOut:true);
        }
        public void SceneLoaded()
        {
            isSceneLoading = false;
        }

        public bool SceneLoad(string sceneName, bool isLoading = true)
        {
            if (isSceneLoading) return false;
            string pattern = @"^Sector(\d+)$";

            // 정규 표현식에 일치하는지 확인
            Match match = Regex.Match(sceneName, pattern);
            if (match.Success)
            {
                // 일치하는 부분이 있다면 숫자를 추출
                string numberPart = match.Groups[1].Value;

                if (int.TryParse(numberPart, out int intValue))
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
                        CameraManager.instance.ToggleCameraFix(false);

                        if (!CurSceneData.isPlayerMustExist)
                        {
                            GameManager.instance.DestroyPlayer();
                        }
                    }

                    LoadingSceneManager.LoadStage(sceneName);
                }, () =>
                {
                    if (GameManager.Scene.IsSector(sceneName))
                    {
                        GameManager.SectorMag.IsFirstSector = true;
                    }
                }, isFadeIn:IsFading(CurSceneData), isFadeOut:false);
            }
            else
            {
                SceneData nextData = GetNextSceneData(sceneName);
                FadeManager.instance.Fading(() =>
                {
                    if (IsSector(sceneName))
                    {
                        GameManager.SectorMag.IsFirstSector = true;
                    }
                    WhenSceneLoadBegin.Invoke(CurSceneData);
                    CurSceneData = nextData;
                    if (GameManager.instance.Player != null)
                    {
                        CameraManager.instance.ToggleCameraFix(false);
                        if (!CurSceneData.isPlayerMustExist)
                        {
                            GameManager.instance.DestroyPlayer();
                        }
                    }

                    SceneManager.LoadScene(sceneName);
                    
                }, isFadeIn:IsFading(CurSceneData), isFadeOut:IsFading(nextData));
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
            SceneData newScene = GetNextSceneData(scene2.name);
            if (newScene.sceneType == SceneType.Loading)
            {
                LoadingSceneManager.LoadLoadingScene();
            }

            WhenSceneLoaded.Invoke(newScene);
        }

    }
}