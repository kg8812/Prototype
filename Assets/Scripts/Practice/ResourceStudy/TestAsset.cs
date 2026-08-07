using System;
using System.Collections.Generic;
using UnityEngine;

public class TestAsset : MonoBehaviour
{
    class TestClass
    {
        public string name;

        public TestClass(String name)
        {
            this.name = name;
            Debug.Log(name + "생성됨");
        }
    }
    private RefCountedCache<string, TestClass> refCache;

    private int count;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        refCache = new RefCountedCache<string, TestClass>(x =>
        {
            count++;
            Debug.Log(x.name + "파기됨!" + " 파기 카운트 : " + count);
        });
        var obj = refCache.Acquire("slime", () => new TestClass("slime"));
        var temp = refCache.Acquire("slime", () => new TestClass("slime"));
        Debug.Log(ReferenceEquals(obj , temp) ? "A-1 오브젝트 비교 테스트 통과" : "A-1 오브젝트 비교 테스트 실패");
        
        Debug.Log(refCache.GetRefCount("slime") == 2 ? "A-2 : RefCount 2 테스트 통과" : "A-2 : RefCount 2 테스트 실패");
        refCache.Release("slime");
        
        Debug.Log(refCache.AssetCount == 1 ? "B-1 : AssetCount 1 테스트 통과" : "B-1 : AssetCount 1 테스트 실패");
        Debug.Log(refCache.GetRefCount("slime") == 1 ? "B-2 : RefCount 1 테스트 통과" : "B-2 : RefCount 1 테스트 실패");

        refCache.Release("slime");
        Debug.Log(refCache.AssetCount == 0 ? "C-1 : AssetCount 0 테스트 통과" : "C-1 : AssetCount 0 테스트 실패");
        Debug.Log(refCache.GetRefCount("slime") == 0 ? "C-2 : RefCount 0 테스트 통과" : "C-2 : RefCount 0 테스트 실패");
        var obj2 = refCache.Acquire("slime", () => new TestClass("slime"));
        
        Debug.Log(!ReferenceEquals(obj,obj2) ? "D : 오브젝트 비교 테스트 통과" : "D : 오브젝트 비교 테스트 실패");

        refCache.Clear();
        Debug.Log(count == 2 && refCache.AssetCount == 0
            ? "E : Clear 테스트 통과"
            : "E : Clear 테스트 실패");

        try
        {
            refCache.Release("slime");
            Debug.Log("F : null Release 테스트 실패");
        }
        catch(KeyNotFoundException)
        {
            Debug.Log("F : null Release 테스트 성공");
        }

        try
        {
            refCache.Acquire("slime", () => null);
            Debug.Log("G : null acquire 테스트 실패");
        }
        catch(ArgumentException)
        {
            Debug.Log("G : null acquire 테스트 성공");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
