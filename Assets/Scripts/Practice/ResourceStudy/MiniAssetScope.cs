using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MiniAssetScope : IDisposable
{
    readonly struct AssetKey : IEquatable<AssetKey> 
    {
        readonly string address;
        private readonly Type type;
        
        public AssetKey(string address, Type type)
        {
            this.address = address;
            this.type = type;
        }
        
        public bool Equals(AssetKey other)
        {
            return other.address == address && other.type == type;
        }

        public override bool Equals(object obj)
        {
            return obj is AssetKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(address, type);
        }
    }
    private readonly Dictionary<AssetKey, AsyncOperationHandle> _handles = new Dictionary<AssetKey, AsyncOperationHandle>();
    private int loadCount;
    private int releaseCount;
    
    public int LoadCount => loadCount;
    public int ReleaseCount => releaseCount;

    public int handleCount => _handles.Count;
    
    private bool _disposed;

    void Release(AsyncOperationHandle handle)
    {
        Addressables.Release(handle);
        releaseCount++;
    }
    
    public async Awaitable<T> LoadAsync<T>(string address) where T : UnityEngine.Object
    {
        AssetKey key = new AssetKey(address, typeof(T));

        if (!_handles.TryGetValue(key, out var handle))
        {
            handle = Addressables.LoadAssetAsync<T>(address);
            loadCount++;
            _handles.Add(key, handle);
        }
        
        while (handle.IsValid() && !handle.IsDone)
        {
            await Awaitable.NextFrameAsync();
        }

        if (_disposed)
        {
            if(handle.IsValid()) Release(handle);
            return null;
        }
        if (handle.IsValid() && handle.Status == AsyncOperationStatus.Succeeded)
        {
            return (T)handle.Result;
        }

        if (handle.IsValid())
        {
            _handles.Remove(key);
            Release(handle);
        }
        
        Debug.LogWarning($"{address} 로드에 실패했습니다");
        return null;
    }
    public void Dispose()
    {
        var list = _handles.Values.ToList();
        _handles.Clear();
        
        foreach (var handle in list)
        {
            if (handle.IsValid())
            {
                Release(handle);
            }
        }

        _disposed = true;
    }
}
