using System.Collections;
using Apis.Managers;
using Save.Schema;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Apis
{
    public class IntroManager: MonoBehaviour
    {
        [SerializeField] private float logoDelay = 3f;
        [SerializeField] private UI_Intro IntroUI;
        
        private void Start()
        {
            IntroUI.introManager = this;
            if (DataAccess.GameData.Data.IsFirstGame)
            {
                IntroUI.TryActivated();
                IntroUI.OnDeactivated.AddListener(() =>
                {
                    StartCoroutine(ProgressAfterTime());
                });
            }
            else
            {
                StartCoroutine(ProgressAfterTime());
            }
        }


        private IEnumerator ProgressAfterTime()
        {
            yield return new WaitForSecondsRealtime(logoDelay);
            
        }

        public void StartGame()
        {
            GameManager.Scene.SceneLoad(Define.SceneNames.TitleSceneName, false);
        }
    }
}