using System;
using UnityEngine;

public class MiniScopeTest : MonoBehaviour
{
    private MiniAssetScope scope;

    private void Start()
    {
        scope = new();
        var triangle = scope.Load<GameObject>("Triangle");
        Debug.Log(triangle);
        var triangle2 = scope.Load<GameObject>("Triangle");
        scope.Dispose();
        Debug.Log(ReferenceEquals(triangle,triangle2));
        Debug.Log(scope.LoadCount + ","+ scope.ReleaseCount);
        Debug.Log(scope.handleCount);
        
        
    }
}
