using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EPlayerCooldown
{
    Dash,
    Jump,
    Skill,
    DashToAttack,
    DashToJump,
    JumpToAttack,
}

public class PlayerCooldown
{
    private readonly Dictionary<EPlayerCooldown, Coroutine> CdCoroutineDict;
    private readonly Dictionary<EPlayerCooldown, bool> CdDict;
    private readonly Dictionary<EPlayerCooldown, UnityAction> CdEventDict;
    private readonly Player _player;

    public PlayerCooldown(Player player)
    {
        _player = player;
        CdCoroutineDict = new Dictionary<EPlayerCooldown, Coroutine>();
        CdDict = new Dictionary<EPlayerCooldown, bool>();
        CdEventDict = new Dictionary<EPlayerCooldown, UnityAction>();
        Init();
    }

    public void StartCd(EPlayerCooldown state, float time = -1)
    {
        var cd = CdCoroutineDict[state];

        if (cd != null)
            GameManager.instance.StopCoroutineWrapper(cd);

        // 음수 time이면 StopCD 호출 전까지 계속 false
        if (time < 0)
        {
            CdDict[state] = false;
            return;
        }


        IEnumerator cdCoroutine()
        {
            CdDict[state] = false;
            yield return new WaitForSeconds(time);
            CdDict[state] = true;
            CdCoroutineDict[state] = null;
            if (CdEventDict.TryGetValue(state, out var onFinish))
            {
                onFinish.Invoke();
                CdEventDict.Remove(state);
            }
        }

        CdCoroutineDict[state] = GameManager.instance.StartCoroutineWrapper(cdCoroutine());
    }

    public void StartCd(EPlayerCooldown state, UnityAction onFinish, float time = -1)
    {
        StartCd(state, time);

        if (!CdEventDict.TryGetValue(state, out var events))
            CdEventDict.Add(state, onFinish);
        else
            CdEventDict[state] += onFinish;
    }

    public void Init()
    {
        CdCoroutineDict.Clear();
        foreach (EPlayerCooldown cd in Enum.GetValues(typeof(EPlayerCooldown)))
        {
            CdCoroutineDict.Add(cd, null);
            CdDict.Add(cd, true);
        }
    }

    public void StopCd(EPlayerCooldown state)
    {
        if (CdDict[state]) return;

        if (CdCoroutineDict[state] != null)
            GameManager.instance.StopCoroutine(CdCoroutineDict[state]);

        CdCoroutineDict[state] = null;
        CdDict[state] = true;
        if (CdEventDict.ContainsKey(state)) CdEventDict.Remove(state);
    }

    public void CompleteCd(EPlayerCooldown state)
    {
        if (CdDict[state]) return;

        if (CdCoroutineDict[state] != null)
            GameManager.instance.StopCoroutine(CdCoroutineDict[state]);

        CdCoroutineDict[state] = null;
        CdDict[state] = true;

        if (CdEventDict.TryGetValue(state, out var onFinish))
        {
            onFinish.Invoke();
            CdEventDict.Remove(state);
        }
    }

    public bool GetCd(EPlayerCooldown state)
    {
        return CdDict[state];
    }
}