using Apis;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetMonsterIsPublicEditor : EditorWindow
{
    [MenuItem("Tools/몬스터 데이터 초기화")]
    public static void SetMonsterExistSetting()
    {
        // Get all GameObjects in the current scene
        var allObjects = FindObjectsOfType<GameObject>();

        var count = 0;

        foreach (var go in allObjects)
        {
            // Get all Monster scripts attached to the GameObject
            var monsters = go.GetComponents<Monster>();

            foreach (var monster in monsters)
                // Set the isPublic variable to true
                if (monster.isAlreadyCreated != true)
                {
                    monster.isAlreadyCreated = true;
                    count++;

                    // Mark the object as dirty to enable saving the changes
                    EditorUtility.SetDirty(monster);
                }
        }

        // Save the scene
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log($"Set isPublic to true for {count} Monster scripts.");
    }
}