using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddrLoader : MonoBehaviour
{
    async void Start()
    {
        try
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("Triangle");
            while (!handle.IsDone) await Awaitable.NextFrameAsync();
            var labelHandle = Addressables.LoadAssetsAsync<GameObject>("Triangles",null);
            while (!labelHandle.IsDone) await Awaitable.NextFrameAsync();
            Debug.Log(labelHandle.Result.Count);
            foreach (var x in labelHandle.Result)
            {
                Instantiate(x);
            }
            Addressables.Release(labelHandle);
            Debug.Log(handle.IsValid());
            Debug.Log("triangle :" + handle.Result);
            Debug.Log("label :" + labelHandle.Result);

        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }

    
    IEnumerator coroutineTest()
    {
        yield return new WaitForSeconds(1f);

        Debug.Log("Run");
    }

    // Update is called once per frame
    void Update()
    {
    }
}