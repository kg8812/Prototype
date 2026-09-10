using System;
using System.Threading;
using Default;
using UnityEngine;

public class MiniScopeTest : MonoBehaviour
{
    private async void Start()
    {
        // 수정 검증: 씨 전환 중에 끝난 로드가 죽은 스코프에 등록되는가
        var old = AssetRegistry.Scene;

        var a = old.LoadAsync<GameObject>("Triangle");
        await Awaitable.NextFrameAsync();

        AssetRegistry.ReleaseScene();
        Debug.Log($"[실물] 전환 직후 old={old.LoadedCount}");

        var r = await a;

        Debug.Log($"[실물] 복귀 후 old={old.LoadedCount} / new={AssetRegistry.Scene.LoadedCount} / r==null:{r == null}");
    }
}
