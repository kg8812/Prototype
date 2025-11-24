using System;
using Apis;
using Apis.DataType;
using Save.Schema;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class StatueOfProtection : MonoBehaviour, IOnInteract
{
    public Func<bool> InteractCheckEvent { get; set; }

    [LabelText("버프 지속시간")] public float duration;
    [LabelText("공격력 증가량")] public float atk;
    [LabelText("방어력 증가량")] public float def;
    [LabelText("결속력 증가량")] public float cd;

    private Buff atkBuff;
    private Buff defBuff;
    private Buff cdBuff;

    private Buff[] buffs = new Buff [3];
    private bool isUsed;
    
    bool Check()
    {
        return DataAccess.LobbyData.IsOpen(501) && !isUsed;
    }
    private void Start()
    {
        InteractCheckEvent += Check;
        isUsed = false;
    }

    public void OnInteract()
    {
        if (atkBuff == null)
        {
            BuffDataType data = new BuffDataType(SubBuffType.Buff_Atk)
            {
                buffPower = new[]{atk}, buffCategory = 1, buffDuration = duration,
                buffDispellType = 1, applyType = 0, buffMaxStack = 1, valueType = ValueType.Ratio, showIcon = true,
                buffIconPath = "BuffIcon_atkUp"
            };
            atkBuff = new(data, GameManager.instance.Player);
        }
        if (defBuff == null)
        {
            BuffDataType data = new BuffDataType(SubBuffType.Buff_Def)
            {
                buffPower = new[]{def}, buffCategory = 1, buffDuration = duration,
                buffDispellType = 1, applyType = 0, buffMaxStack = 1, valueType = ValueType.Ratio, showIcon = true,
                buffIconPath = "BuffIcon_defUp"
            };
            defBuff = new(data, GameManager.instance.Player);
        }
        if (cdBuff == null)
        {
            BuffDataType data = new BuffDataType(SubBuffType.Buff_CD)
            {
                buffPower = new[]{cd}, buffCategory = 1, buffDuration = duration,
                buffDispellType = 1, applyType = 0, buffMaxStack = 1, valueType = ValueType.Ratio, showIcon = true,
            };
            cdBuff = new(data, GameManager.instance.Player);
        }

        buffs[0] = atkBuff;
        buffs[1] = defBuff;
        buffs[2] = cdBuff;
        buffs[Random.Range(0,buffs.Length)]?.AddSubBuff(GameManager.instance.Player,null);

        isUsed = true;
    }
}
