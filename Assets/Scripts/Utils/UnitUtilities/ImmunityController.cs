using System;
using System.Collections.Generic;

// 면역 관리 클래스
public class ImmunityController
{
    private readonly Dictionary<string, HashSet<Guid>> lists = new();

    // 새로운 타입 추가 (무적, 버프 면역 등 필요할때마다 추가하세요)
    public bool MakeNewType(string type)
    {
        return lists.TryAdd(type, new HashSet<Guid>());
    }

    // 특정 타입 카운트 추가
    public Guid AddCount(string type)
    {
        if (!lists.TryGetValue(type, out var set)) return Guid.Empty;

        var guid = Guid.NewGuid();

        set.Add(guid);

        return guid;
    }

    // 특정 타입 카운트 제거
    public void MinusCount(string type, Guid guid)
    {
        if (!lists.TryGetValue(type, out var set)) return;

        set.Remove(guid);
    }

    // 특정 타입 면역 제거
    public void MakeCountToZero(string type)
    {
        if (!lists.TryGetValue(type, out var set)) return;

        set.Clear();
    }

    // 면역 확인
    public bool IsImmune(string type)
    {
        var b = lists.TryGetValue(type, out var set) && set.Count > 0;
        return b;
    }

    public bool Contains(string type)
    {
        return lists.ContainsKey(type);
    }
}