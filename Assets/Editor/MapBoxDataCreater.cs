using System.Collections.Generic;
using System.IO;
using Apis;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class MapBoxDataCreater : EditorWindow
{

    [MenuItem("Tools/mapbox data 생성")]
    public static void ProcessSectorsInternal()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("[ERROR] 에디터 모드에서만 실행 가능합니다!");
            return;
        }

        List<GameObject> sectorSceneNames = new List<GameObject>();

        // 1. 현재 씬에서 "Sector{sectorId}" 패턴을 가진 오브젝트 찾기
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.StartsWith("SectorLC_"))
            {
                sectorSceneNames.Add(obj);
            }
        }

        foreach (GameObject sectorScene in sectorSceneNames)
        {
            GameObject child = sectorScene.transform.Find("SectorLoader").gameObject;
            if(child == null) continue;
            ProcessSector(child, sectorScene.name.Replace("SectorLC_", ""));
        }
    }

    private static void ProcessSector(GameObject sectorScene, string sectorId)
    {
        // 2. 씬 로드 (에디터 모드에서는 OpenScene 사용)
        string scenePath = $"Assets/01_Scenes/Sectors/{sectorId[0]}Stage/Sector{sectorId}.unity";
        if (!File.Exists(scenePath))
        {
            Debug.LogError($"[ERROR] 씬 파일이 존재하지 않음: {scenePath}");
            return;
        }

        Scene loadedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

        // 3. Sector 스크립트를 가진 오브젝트 찾기
        GameObject[] rootObjects = loadedScene.GetRootGameObjects();
        Sector targetSector = null;

        foreach (GameObject obj in rootObjects)
        {
            targetSector = obj.GetComponent<Sector>();
            if (targetSector != null)
                break;
        }

        if (targetSector == null)
        {
            Debug.LogError($"[ERROR] {sectorScene}에서 Sector 컴포넌트가 없음!");
            return;
        }

        // 4. MapDataMaker 컴포넌트 추가 및 설정
        MapDataMaker mapDataMaker = targetSector.gameObject.GetComponent<MapDataMaker>();
        if (mapDataMaker == null)
        {
            mapDataMaker = targetSector.gameObject.AddComponent<MapDataMaker>();
        }
        BoxCollider2D collider = sectorScene.GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            Debug.LogError($"오브젝트 '{sectorScene}'에 BoxCollider2D가 없습니다.");
            return;
        }
        
        mapDataMaker.size = collider.size;
        mapDataMaker.position = collider.transform.position;

        // 5. ScriptableObject 동적 생성 및 저장
        
        string savePath = $"Assets/06_ScriptableObjects/Datas/Map/MapBox/MapBox_{sectorId}.asset";
        MapBoxPermanentData mapBoxData;
        
        mapBoxData = AssetDatabase.LoadAssetAtPath<MapBoxPermanentData>(savePath);
        if (mapBoxData == null)
        {
            mapBoxData = ScriptableObject.CreateInstance(typeof(MapBoxPermanentData)) as MapBoxPermanentData;
            AssetDatabase.CreateAsset(mapBoxData, savePath);
        }
        
        mapBoxData.mapBoxId = int.Parse(sectorId);
        mapBoxData.mapBoxData.size = collider.size;
        mapBoxData.mapBoxData.position = collider.transform.position;
        EditorUtility.SetDirty(mapBoxData);
        AssetDatabase.SaveAssets();

        Debug.Log($"[SUCCESS] MapBoxPermanentData 저장 완료: {savePath}");

        // 6. 씬 저장
        EditorSceneManager.MarkSceneDirty(loadedScene);
        bool saveSuccess = EditorSceneManager.SaveScene(loadedScene);
        if (saveSuccess)
        {
            Debug.Log($"[SUCCESS] {sectorScene} 씬 저장 완료");
        }
        else
        {
            Debug.LogError($"[ERROR] {sectorScene} 씬 저장 실패!");
        }

        // 7. 씬 언로드
        EditorSceneManager.CloseScene(loadedScene, true);
    }
}
