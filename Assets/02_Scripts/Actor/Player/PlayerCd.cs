using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum EPlayerCd
{
    Dash,
    Jump,
    AirAttack,
    Skill,
    DashToAttack,
    DashToJump,
    JumpToAttack,
    AttackComboDelay,
    AttackAfterDelay
}

public class PlayerCd
{
    private readonly bool[] AirAttackCd;
    private readonly Dictionary<EPlayerCd, Coroutine> CdCoroutineDict;
    private readonly Dictionary<EPlayerCd, bool> CdDict;
    private readonly Dictionary<EPlayerCd, UnityAction> CdEventDict;
    private readonly Player _player;

    public PlayerCd(Player player)
    {
        _player = player;
        CdCoroutineDict = new Dictionary<EPlayerCd, Coroutine>();
        CdDict = new Dictionary<EPlayerCd, bool>();
        CdEventDict = new Dictionary<EPlayerCd, UnityAction>();
        AirAttackCd = new bool[4];
        Init();
    }

    public void StartCd(EPlayerCd state, float time = -1)
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

    public void StartCd(EPlayerCd state, UnityAction onFinish, float time = -1)
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
        foreach (EPlayerCd cd in Enum.GetValues(typeof(EPlayerCd)))
        {
            CdCoroutineDict.Add(cd, null);
            CdDict.Add(cd, true);
        }

        for (var i = 0; i < AirAttackCd.Length; i++) AirAttackCd[i] = false;
    }

    public void StopCd(EPlayerCd state)
    {
        if (CdDict[state]) return;

        if (CdCoroutineDict[state] != null)
            GameManager.instance.StopCoroutine(CdCoroutineDict[state]);

        CdCoroutineDict[state] = null;
        CdDict[state] = true;
        if (CdEventDict.ContainsKey(state)) CdEventDict.Remove(state);
    }

    public void CompleteCd(EPlayerCd state)
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

    public bool GetCd(EPlayerCd state)
    {
        return CdDict[state];
    }

    /* 공중 공격 쿨다운용 */

    #region AirAttackCd

    public void SetAirAttackCd(int index, bool isCd)
    {
        if (index > 3) return;

        AirAttackCd[index] = isCd;
    }

    public void ResetAirAttackCd()
    {
        for (var i = 0; i < AirAttackCd.Length; i++) AirAttackCd[i] = false;
    }

    public bool GetAirAttackCd(int index)
    {
        if (!_player.onAir) return true;

        var MaxAirAtk = _player.animator.GetInteger("MaxAirAtk");
        return MaxAirAtk == 0 || (_player.weaponAtkInfo.atkCombo != MaxAirAtk
                                  && !AirAttackCd[index]);
    }

    public bool HasAirAttackCd(int index)
    {
        return AirAttackCd[index];
    }

    #endregion
}