using System;
using System.Collections;
using System.Threading;
using Apis.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddrLoader : MonoBehaviour
{
    private CancellationTokenSource _source;
    private static bool isStarted = false;
    private static AsyncOperationHandle<GameObject> handle;
    private static bool isSceneloaded = false;
    async void Start()
    {
        if (isStarted)
        {
            Debug.Log(handle.IsValid());
            Debug.Log(handle.IsDone);
            Debug.Log(handle.Status);
            Debug.Log(handle.Result);
            return;
        }

        isStarted = true;
        handle = Addressables.LoadAssetAsync<GameObject>("Triangle");
        try
        {
            await AwaitTest(destroyCancellationToken);
        }
        catch (OperationCanceledException e)
        {
            Debug.Log(e.Message);
        }
    }

    async Awaitable AwaitTest(CancellationToken token)
    {
        for (int i = 0; i < 10; i++)
        {
            await Awaitable.NextFrameAsync();
            token.ThrowIfCancellationRequested();
            Debug.Log($"Tick :  {GetInstanceID()}  , {i + 1}");
        }

        Debug.Log("종료");
    }

    IEnumerator coroutineTest()
    {
        try
        {
            for (int i = 0; i < 10; i++)
            {
                yield return null;
                Debug.Log(Time.frameCount);
            }

            Debug.Log("종료");
            Debug.Log(this == null);
            Debug.Log(this is null);
            Debug.Log(gameObject == null);
            Debug.Log(transform.position);
        }
        finally
        {
            Debug.Log("Finally");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount > 5 && !isSceneloaded)
        {
            //Debug.Log("삭제 프레임 : " + Time.frameCount);
            //Destroy(gameObject);
            Debug.Log("취소 프레임 : " + Time.frameCount);
            //_source.Cancel();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            isSceneloaded = true;
        }
    }
}