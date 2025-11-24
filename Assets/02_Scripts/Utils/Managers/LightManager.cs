using Apis.Managers;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Managers
{
    public class LightManager: SingletonPersistent<LightManager>
    {
        [SerializeField] private Light2D playerFollowLight;

        protected override void Awake()
        {
            base.Awake();
            GameManager.Scene.WhenSceneLoaded.AddListener(LightingPerScene);
        }

        private void LightingPerScene(SceneData sceneData)
        {
            if (!sceneData.isPlayerMustExist)
            {
                playerFollowLight.enabled = false;
            }
            else
            {
                playerFollowLight.enabled = true;
            }
        }
    }
}