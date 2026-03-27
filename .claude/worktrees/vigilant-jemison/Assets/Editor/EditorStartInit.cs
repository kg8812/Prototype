using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class EditorStartInit
{
    static EditorStartInit()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        var pathOfScene = "";
        if (!sceneName.StartsWith("Test"))
        {
            Initializer.staticSceneName = sceneName == "Init" ? "" : sceneName;
            pathOfScene = EditorBuildSettings.scenes[0].path; // 씬 번호를 넣어주자.
        }
        else
        {
            pathOfScene = SceneManager.GetActiveScene().path;
        }

        var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(pathOfScene);
        EditorSceneManager.playModeStartScene = sceneAsset;
    }
}