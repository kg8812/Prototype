using System;
using System.Collections.Generic;
using Apis;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class PracticePuppet : Actor
{
    [SerializeField]private TextMeshPro dmgText;
    [SerializeField]private TextMeshPro groggyText;
    
    public override void IdleOn()
    {
    }

    public override void AttackOn()
    {
    }

    public override void AttackOff()
    {
    }
    
    private class DamageRecord
    {
        public float time;
        public float damage;
        public float groggy;
        public DamageRecord(float time, float damage,float groggy)
        {
            this.time = time;
            this.damage = damage;
            this.groggy = groggy;
        }
    }

    [LabelText("dps 계산 시간")][InfoBox("5초 입력시 지난 5초동안 입힌 데미지의 dps가 나옴")]
    [SerializeField] private float dpsWindow = 5f;
    private List<DamageRecord> damageRecords = new();
    private float currentDPS = 0f;
    private float currentGPS = 0f;

    protected override void Update()
    {
        dmgText.text = $"dps : {currentDPS:F2}";
        groggyText.text = $"groggy : {currentGPS:F2}";
    }

    private void UpdateDPS()
    {
        float currentTime = Time.time;
        float startTime = currentTime - dpsWindow;

        damageRecords.RemoveAll(record => record.time < startTime);

        float totalDamage = 0;
        float totalGroggy = 0;
        foreach (var record in damageRecords)
        {
            totalDamage += record.damage;
            totalGroggy += record.groggy;
        }

        currentDPS = totalDamage / dpsWindow;
        currentGPS = totalGroggy / dpsWindow;
    }
    
    public override float CurHp
    {
        get => base.CurHp;
        set
        {
            base.CurHp = value;

            curHp = MaxHp;
        }
    }

    public override float OnHit(EventParameters parameters)
    {
        float dmg = base.OnHit(parameters);
        if (dmg > 0)
        {
            damageRecords.Add(new DamageRecord(Time.time, dmg, parameters.atkData.groggyAmount));
            UpdateDPS();
        }

        return dmg;
    }
}
