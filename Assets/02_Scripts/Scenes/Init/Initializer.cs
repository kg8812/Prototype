using System.Collections.Generic;
using Apis;
using Apis;
using Directing;
using Managers;
using Save.Schema;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Initializer : SerializedMonoBehaviour
{
    public static string staticSceneName = "";
    [Title("테스트용")] [SerializeField] private string sceneName;
    
    private void Awake()
    {
        Screen.SetResolution(1920,1080,true);
        Addressables.InitializeAsync();
        GameManager.Item.LoadItems();
        Map.instance.DataInit();
    }

    private void Start()
    {
        GameManager.UI.CreateUI("UI_MainHud", UIType.Main);
        GameManager.UI.CreateUI("UI_MiniMap", UIType.Main);
        GameManager.Save.LoadExceptSlot();

        if (staticSceneName != "")
        {
            sceneName = staticSceneName;
        }

        staticSceneName = "";

        GameManager.Scene.SceneLoad(sceneName, false);
    }

#if UNITY_EDITOR

    [MenuItem("AssetDataBase/DataClear")]
    #endif
    [Button(ButtonSizes.Large),GUIColor(0.8f,0,0)]
    public static void DataClear()
    {
        SaveManager.ClearDataFiles();
        Debug.Log("기존의 데이터가 삭제되었습니다.");
    }
}