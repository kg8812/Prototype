using Save.Schema;
using UnityEngine;

namespace Scenes.Tutorial
{
    public class TutorialManager: MonoBehaviour
    {
        public void ToStage()
        {
            GameManager.Scene.SceneLoad(Define.SceneNames.MainWorldSceneName);
        }
    }
}