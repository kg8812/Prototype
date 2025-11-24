using Apis;
using Apis.CommonMonster2;
using Default;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SetMonsterIsPublicEditor : EditorWindow
{
    [MenuItem("Tools/몬스터 데이터 초기화")]
    public static void SetMonsterExistSetting()
    {
        // Get all GameObjects in the current scene
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        
        int count = 0;

        foreach (GameObject go in allObjects)
        {
            // Get all Monster scripts attached to the GameObject
            Monster[] monsters = go.GetComponents<Monster>();
            
            foreach (Monster monster in monsters)
            {
                // Set the isPublic variable to true
                if (monster.isAlreadyCreated != true)
                {
                    monster.isAlreadyCreated = true;
                    count++;
                    
                    // Mark the object as dirty to enable saving the changes
                    EditorUtility.SetDirty(monster);
                }
            }
        }

        // Save the scene
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        Debug.Log($"Set isPublic to true for {count} Monster scripts.");
    }
    
    
}