using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Apis;
using Apis;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace Apis
{
    public class PickUpStringToIder : EditorWindow
    {
        private static Dictionary<string, int> weaponNameToId = new()
        {
            { "미약한 온기의 검", 3001 },
            { "미약한 온기의 단검", 3002 },
            { "미약한 온기의 대검", 3003 },
            { "미약한 온기의 망치", 3004 },
            { "미약한 온기의 낫", 3005 },
            { "미약한 온기의 창", 3006 },
            { "미약한 온기의 지팡이", 3007 },
            { "미약한 온기의 오브", 3008 },
            { "미약한 온기의 총", 3009 },
            { "미약한 온기의 장총", 3010 },
            { "수룡음", 3011 },
            { "거대 롤리팝", 3012 },
            { "사탕 막대", 3013 },
            { "싸대기 방패", 3014 },
            { "망냥냥의 삼정령", 3015 },
            { "데운 귤이 자라는 나무", 3016 },
            { "판타스틱 던", 3017 },
            { "염소희", 3018 },
            { "리드보컬", 3019 },
            { "조각칼", 3020 },
            { "성기사의 검", 3021 },
            { "뇽파검", 3022 },
            { "바벨", 3023 },
            { "철근", 3024 },
            { "뼈 작살", 3025 },
            { "사제의 지팡이", 3026 },
            { "오라버거 스나이퍼", 3027 },
            { "봉인 풀린 주문서", 3028 },
            { "용과같이", 3029 },
            { "욕망의 검", 3030 },
            { "사냥꾼의 총", 3031 },
            { "얼어붙은 깃발", 3032 },
            { "총검", 3033 },
            // { "총검", 3034 },
            { "태풍의 낫", 3035 },
            { "기계 지팡이", 3036 },
            { "희망의 검", 3037 },
            { "역병 도끼", 3038 },
            { "황실 금관악기", 3039 },
            { "검은 장미 글러브", 3040 },
            { "킹구의 검", 3041 },
            { "수문장의 창", 3042 },
            { "중력의 샘", 3043 },
            { "문단속 열쇠", 3044 },
            { "마법의 다이어리", 3045 },
            { "아이네의 지팡이", 3046 },
            { "징버거의 붓", 3047 },
            { "릴파의 총", 3048 },
            { "주르르의 낫", 3049 },
            { "고세구의 클로", 3050 },
            { "비챤의 검", 3051 },
            { "릴파의 샷건", 3052 },
            { "릴파의 장검", 3053 }
        };
        [MenuItem("Tools/PickupStringToIder")]
        public static void Active()
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
                if (obj.name.StartsWith("Sector"))
                {
                    sectorSceneNames.Add(obj);
                }
            }

            foreach (GameObject sectorScene in sectorSceneNames)
            {
                ProcessSector(sectorScene);
            }
        }

        private static void ProcessSector(GameObject sectorScene)
        {
            string sectorId = sectorScene.name.Replace("Sector", "");

            // 2. 씬 로드 (에디터 모드에서는 OpenScene 사용)
            string scenePath = $"Assets/01_Scenes/Sectors/{sectorId[0]}Stage/Sector{sectorId}.unity";
            if (!File.Exists(scenePath))
            {
                Debug.LogError($"[ERROR] 씬 파일이 존재하지 않음: {scenePath}");
                scenePath = $"Assets/01_Scenes/Sectors/Sector{sectorId}.unity";
                if (!File.Exists(scenePath))
                {
                    Debug.LogError($"[ERROR] 씬 파일이 존재하지 않음: {scenePath}");
                    return;
                }
            }

            Scene loadedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

            // 3. Sector 스크립트를 가진 오브젝트 찾기
            GameObject[] rootObjects = loadedScene.GetRootGameObjects();
            
            // TODO
            // 3. Sector 스크립트를 가진 오브젝트 찾기

// 4. 모든 Weapon_PickUp 오브젝트 탐색
            int fixedCount = 0;
            foreach (GameObject root in rootObjects)
            {
                Acc_PickUp[] pickups = root.GetComponentsInChildren<Acc_PickUp>(true);
                foreach (var pickup in pickups)
                {
                    if (pickup.accId == 0)
                    {
                        fixedCount++;
                    }
                    
                }
            }
            if(fixedCount > 0)
                Debug.Log($"[INFO] {loadedScene.name} 씬에서 {fixedCount}개의 Acc_PickUp 를 찾았습니다.");
            
            fixedCount = 0;
            foreach (GameObject root in rootObjects)
            {
                ActiveSkill_PickUp[] pickups = root.GetComponentsInChildren<ActiveSkill_PickUp>(true);
                foreach (var pickup in pickups)
                {
                    if (pickup.activeSkillItemId == 0)
                    {
                        fixedCount++;
                    }
                    
                }
            }
            if(fixedCount > 0)
                Debug.Log($"[INFO] {loadedScene.name} 씬에서 {fixedCount}개의 ActiveSkill_PickUp 를 찾았습니다.");
            
            fixedCount = 0;
            foreach (GameObject root in rootObjects)
            {
                EtcItemPickUp[] pickups = root.GetComponentsInChildren<EtcItemPickUp>(true);
                foreach (var pickup in pickups)
                {
                    if (pickup.itemId == 0)
                    {
                        fixedCount++;
                    }
                    
                }
            }
            if(fixedCount > 0)
                Debug.Log($"[INFO] {loadedScene.name} 씬에서 {fixedCount}개의 EtcItemPickUp 를 찾았습니다.");

            // 6. 씬 저장
            // EditorSceneManager.MarkSceneDirty(loadedScene);
            // bool saveSuccess = EditorSceneManager.SaveScene(loadedScene);
            // if (saveSuccess)
            // {
            //     Debug.Log($"[SUCCESS] {sectorScene} 씬 저장 완료");
            // }
            // else
            // {
            //     Debug.LogError($"[ERROR] {sectorScene} 씬 저장 실패!");
            // }
            //
            // // 7. 씬 언로드
            EditorSceneManager.CloseScene(loadedScene, true);
        }
    }
}