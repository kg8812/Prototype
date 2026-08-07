using System;
using System.Collections.Generic;
using System.Linq;

public class RefCountedCache<TKey,TValue> where TValue : class
{
    
    class AssetData
    {
        public TValue asset;
        public int refCount;
    }
    private readonly Dictionary<TKey, AssetData> _refDict = new();

    private readonly Action<TValue> _onDestroy;
    
    // 카운트가 0이 됐을 때 실제 파기를 위임받는 콜백.
    // 캐시 자신은 "무엇을 파기하는지" 몰라야 한다.
    public RefCountedCache(Action<TValue> onDestroy)
    {
        if (onDestroy == null) throw new ArgumentNullException(nameof(onDestroy));
        _onDestroy = onDestroy;
    }

    // 없으면 factory()로 만들고, 있으면 기존 것을 반환. 어느 쪽이든 카운트 +1.
    
    public TValue Acquire(TKey key, Func<TValue> factory)
    {
        if (_refDict.TryGetValue(key, out var value))
        {
            value.refCount++;
            return value.asset;
        }
        TValue asset = factory();

        if (asset == null)
        {
            throw new ArgumentException("팩토리가 null을 반환합니다.");
        }
        
        _refDict[key] = new AssetData {asset = asset, refCount = 1};

        return asset;
    }

    public void Release(TKey key)
    {
        if (!_refDict.TryGetValue(key, out var value))
        {
            throw new KeyNotFoundException("미등록 에셋을 Release 했습니다.");
        }

        if (--value.refCount != 0) return;
        
        _refDict.Remove(key);
        _onDestroy(value.asset);
    }
    // 카운트 -1. 0이 되면 캐시에서 제거하고 onDestroy 호출.

    public int AssetCount => _refDict.Count; // 살아있는 항목 수

    public int GetRefCount(TKey key)
    {
        if (_refDict.TryGetValue(key, out var value))
        {
            return value.refCount;
        }

        return 0;
    }// 진단용

    public void Clear()
    {
        // 전부 파기함. 파기 안하면 clear의 존재 이유가 없음

        var values = _refDict.Values.ToList();
        _refDict.Clear();
        
        foreach (var value in values)
        {
            _onDestroy(value.asset);
        }
        
    }
}
