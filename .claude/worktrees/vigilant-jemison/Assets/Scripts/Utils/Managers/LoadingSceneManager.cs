using System.Collections;
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
        private static string _nextScene;

        public static void LoadLoadingScene()
        {
            GameManager.instance.StartCoroutineWrapper(LoadScene());
        }

        public static void LoadStage(string sceneName)
        {
            _nextScene = sceneName;
            SceneManager.LoadScene("LoadingScene");
        }

        private static IEnumerator LoadScene()
        {
            var fillAmount = 0f; // 나중에 로딩이 어떻게 될 지 몰라 일단 임시로 만들어둠.
            yield return null;
            var op = SceneManager.LoadSceneAsync(_nextScene);
            op.allowSceneActivation = false;
            var timer = 0.0f;
            while (!op.isDone)
            {
                yield return null;
                timer += Time.unscaledDeltaTime;
                if (op.progress < 0.9f)
                {
                    fillAmount = Mathf.Lerp(fillAmount, op.progress, timer);
                    if (fillAmount >= op.progress) timer = 0f;
                }
                else
                {
                    fillAmount = Mathf.Lerp(fillAmount, 1f, timer);
                    if (fillAmount >= 1.0f)
                    {
                        op.allowSceneActivation = true;
                        yield break;
                    }
                }
            }
        }
    }
}