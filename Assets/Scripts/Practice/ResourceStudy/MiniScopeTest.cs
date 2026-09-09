using System;
using UnityEngine;

public class MiniScopeTest : MonoBehaviour
{
    private MiniAssetScope scope;

    private async void Start()
    {
        scope = new();

        Debug.Log("start: " + Time.frameCount);
        var a = scope.LoadAsync<GameObject>("Triangle");
        
        scope.Dispose();

        Debug.Log("dispose: " + Time.frameCount);
        
        var r = await a;
        
        Debug.Log("return: " + Time.frameCount);
        Debug.Log(r == null);
        Debug.Log(scope.LoadCount + "," + scope.ReleaseCount + "," + scope.handleCount);
    }
}
