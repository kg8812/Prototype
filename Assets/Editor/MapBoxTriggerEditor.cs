using System.Linq;
using Apis.Components;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Apis
{
    public class MapBoxTriggerEditor: EditorWindow
    {
        [MenuItem("Tools/mapbox trigger 세팅")]
        public static void Active()
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if(!obj.name.StartsWith("SectorLC_")) continue;
                GameObject loaderObj = obj.transform.Find("SectorLoader").gameObject;
                Trigger[] triggers = loaderObj.GetComponents<Trigger>();
                if(triggers == null) continue;
                string idSt = obj.name.Replace("SectorLC_", "");
                if (!int.TryParse(idSt, out int mapBoxId)) continue;
                foreach (var trigger in triggers)
                {
                    if (trigger.strategyData.StrategyType == TriggerStrategyType.TS_PlayerEnter)
                    {
                        bool outt = false;
                        foreach (var ac in trigger.activatedDatas)
                        {
                            if (ac.ActivateType == TriggerActivateType.TA_EnterMapBox)
                            {
                                outt = true;
                                break;
                            }
                        }
                        if(outt) break;
                        
                        var newData = new TriggerActivateStruct()
                        {
                            ActivateType = TriggerActivateType.TA_EnterMapBox,
                            mapBoxId = mapBoxId
                        };

                        trigger.activatedDatas = trigger.activatedDatas.Append(newData).ToArray(); // 새 배열 할당
                        EditorUtility.SetDirty(trigger);
                    }
                }
            }
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}