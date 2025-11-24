// using Apis;
// using Apis.CommonMonster2;
// using Default;
// using UnityEditor;
// using UnityEditor.SceneManagement;
// using UnityEngine;
// using UnityEngine.SceneManagement;
//
// namespace Apis
// {
//     public class MonsterChangeEditor: EditorWindow
//     {
//         [MenuItem("Tools/몬스터 Changing")]
//         public static void ChangeMonsters()
//         {
//             // Get all GameObjects in the current scene
//             GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(true);
//         
//             int count = 0;
//
//             foreach (GameObject go in allObjects)
//             {
//                 // Get all Monster scripts attached to the GameObject
//                 CommonMonster[] monsters = go.GetComponents<CommonMonster>();
//             
//                 foreach (CommonMonster monster in monsters)
//                 {
//                     GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(ResourceUtil.Load<CommonMonster2.CommonMonster2>($"Prefabs/Components/MonsterComponent/{monster.monsterId}").gameObject, monster.transform.parent);
//                     CommonMonster2.CommonMonster2 cm = newInstance.GetComponent<CommonMonster2.CommonMonster2>();
//                     cm.isAlreadyCreated = true;
//                     cm.transform.position = new Vector3(monster.transform.position.x, monster.transform.position.y, 0);
//                     cm.transform.localScale = monster.transform.localScale;
//                     EditorUtility.SetDirty(cm);
//                     count++;
//                     DestroyImmediate(monster.gameObject);
//                 }
//             }
//
//             // Save the scene
//             EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
//             Debug.Log($"{count}개의 몬스터가 교체되었습니다.");
//         }
//     }
// }