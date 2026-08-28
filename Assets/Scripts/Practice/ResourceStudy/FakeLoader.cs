using System;
using System.Collections;
using UnityEngine;

public class FakeLoader : MonoBehaviour
{
    async void Start()
    {
        // 성공 케이스
        // string t1 = await AsyncStringTest("Player");
        // Debug.Log(t1);
        
        // test 1

        // AsyncStringTest(null);
        // Debug.Log("호출 다음 줄");
        
        
        // test 2

        // try
        // {
        //     AsyncStringTest(null);
        // }
        // catch
        // {
        //     Debug.Log("string 에러");
        // }
        
        // test 3
        
        // try
        // {
        //     await AsyncStringTest(null);
        // }
        // catch
        // {
        //     Debug.Log("string 에러");
        //     Debug.Log(Time.frameCount);
        // }

        try
        {
            var op = StartLoading(null);
            
            while (!op.isDone)
            {
                await Awaitable.NextFrameAsync();
                Debug.Log("진행률 " + op.progress);
            }
            string result = await op.box;
            Debug.Log("Loaded " + result);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }
    IEnumerator CoroutineTest()
    {
        
        GameObject o = null;
        o.SetActive(true);
        
        Debug.Log("Coroutine : "+ Time.frameCount);
        for (int i = 0; i < 3; i++)
        {
            yield return null;
        }
        Debug.Log("Coroutine : "+ Time.frameCount);
    }

    // private void Update()
    // {
    //     if (Time.frameCount > 5) return;
    //     Debug.Log("Update" + Time.frameCount);
    // }

    class Loader
    {
        public float progress;
        public Awaitable<string> box;
        public bool isDone = false;
        public bool succeeded = false;
    }

    Loader StartLoading(string key)
    {
        Loader  loader = new Loader();

        loader.box = AsyncStringTest(key,loader);

        return loader;
    }
   
    async Awaitable<String> AsyncStringTest(string key,Loader loader)
    {
        try
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("키가 비어있다.", nameof(key));
            }
            
            for (int i = 1; i < 4; i++)
            {
                await Awaitable.NextFrameAsync();
                loader.progress = i / 3f;
            }
            
            loader.succeeded = true;
            return "Loaded : " + key;
        }
        finally
        {
            loader.isDone = true;
        }

    }
    async Awaitable AsyncTest()
    {
        Debug.Log(System.Threading.Thread.CurrentThread.ManagedThreadId);
        Debug.Log("Async : " + Time.frameCount);
        
        await Awaitable.BackgroundThreadAsync();
        
        Debug.Log(System.Threading.Thread.CurrentThread.ManagedThreadId);
        Debug.Log("Async : " + Time.frameCount);
        
        // for (int i = 0; i < 3; i++)
        // {
        //     await Awaitable.NextFrameAsync();
        // }
        
        await Awaitable.MainThreadAsync();
        
        
        Debug.Log("Async : " + Time.frameCount);

    }
}
