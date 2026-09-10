using System;
using System.Threading;
using Default;
using UnityEngine;

public class MiniScopeTest : MonoBehaviour
{
    private MiniAssetScope scope;

    private async void Start()
    {
        // var old = AssetRegistry.Scene;
        //
        // var a = old.LoadAsync<GameObject>("Triangle");
        // await Awaitable.NextFrameAsync();
        //
        // AssetRegistry.ReleaseScene();
        // Debug.Log($"[실물] 전환 직후 old={old.LoadedCount}");
        //
        // var r = await a;                                  // 프레임 5~6: 늦게 복귀
        //
        // Debug.Log($"[실물] 복귀 후 old={old.LoadedCount} / new={AssetRegistry.Scene.LoadedCount} / r==null:{r == null}");

        var mini = new MiniAssetScope();

        var a = mini.LoadAsync<GameObject>("Triangle");
        await Awaitable.NextFrameAsync();

        mini.Dispose();
        Debug.Log($"[미니] 전환 직후 handles={mini.handleCount}");                                                                                                                       
                                                                                                                                                                                   
        var r = await a;                                                                                                                                                                 
                                                                                                                                                                                   
        Debug.Log($"[미니] 복귀 후 handles={mini.handleCount} / load={mini.LoadCount} / release={mini.ReleaseCount} / r==null:{r == null}");  
    }
}