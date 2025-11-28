using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class PracticePuppet : Actor
{
    [SerializeField] private TextMeshPro dmgText;
    [SerializeField] private TextMeshPro Text;

    [LabelText("dps 계산 시간")] [InfoBox("5초 입력시 지난 5초동안 입힌 데미지의 dps가 나옴")] [SerializeField]
    private float dpsWindow = 5f;

    private float currentDPS;
    private float currentGPS;
    private readonly List<DamageRecord> damageRecords = new();

    public override float CurHp
    {
        get => base.CurHp;
        set
        {
            base.CurHp = value;

            curHp = MaxHp;
        }
    }

    protected override void Update()
    {
        dmgText.text = $"dps : {currentDPS:F2}";
        Text.text = $" : {currentGPS:F2}";
    }

    public override void IdleOn()
    {
    }

    public override void AttackOn()
    {
    }

    public override void AttackOff()
    {
    }

    private void UpdateDPS()
    {
        var currentTime = Time.time;
        var startTime = currentTime - dpsWindow;

        damageRecords.RemoveAll(record => record.time < startTime);

        float totalDamage = 0;
        float total = 0;
        foreach (var record in damageRecords) totalDamage += record.damage;

        currentDPS = totalDamage / dpsWindow;
        currentGPS = total / dpsWindow;
    }

    public override float OnHit(EventParameters parameters)
    {
        var dmg = base.OnHit(parameters);
        if (dmg > 0) UpdateDPS();

        return dmg;
    }

    private class DamageRecord
    {
        public readonly float damage;
        public readonly float time;

        public DamageRecord(float time, float damage)
        {
            this.time = time;
            this.damage = damage;
        }
    }
}