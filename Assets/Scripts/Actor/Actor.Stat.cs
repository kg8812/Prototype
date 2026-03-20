using System;
using Apis;
using EventData;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class Actor : IStatUser, IBarrierUser
{
    ActorHpController  _hpController;
    public ActorHpController  HPController => _hpController;
    
    public BarrierCalculator BarrierCalculator => HPController.BarrierCalculator;
    
    [SerializeField] protected StatManager _statManager;

    public virtual float MoveSpeed => StatManager.GetFinalStat(ActorStatType.MoveSpeed);
    public virtual float AtkSpeed => StatManager.GetFinalStat(ActorStatType.AtkSpeed);

    public virtual float Def => StatManager.GetFinalStat(ActorStatType.Def);

    public virtual float CritProb => StatManager.GetFinalStat(ActorStatType.CritProb);
    public virtual float CritDmg => StatManager.GetFinalStat(ActorStatType.CritDmg);

    public virtual float Atk => StatManager.GetFinalStat(ActorStatType.Atk);

    public virtual float CurHp
    {
        get => HPController.CurHp;
        set => HPController.ApplyDmgToTargetValue(value);
    }

    public virtual float MaxHp => StatManager.GetFinalStat(ActorStatType.MaxHp);
    public virtual StatManager StatManager => _statManager;

    public float Barrier => HPController.Barrier;
    public void SetHpWithoutEvent(float hp)
    {
        HPController.SetHpWithoutEvent(hp);
    }

    public event StatManager.StatEvent BonusStatEvent
    {
        add
        {
            StatManager.BonusStatEvent -= value;
            StatManager.BonusStatEvent += value;
        }
        remove => StatManager.BonusStatEvent -= value;
    }

    public void AddBarrier(float amount)
    {
        HPController?.AddBarrier(amount);
    }

    protected void ResetTextVariables()
    {
        HPController.ResetTextVariables();
    }

    #region 대쉬 관련 (임시, 수치 정해지면 actormovement 내에 const로 뺄 듯)

    [TabGroup("기획쪽 수정 변수들/group1", "조작감")] [LabelText("대쉬 모서리 보정 최대 거리")] [SerializeField]
    private float maxEdgeModifier = 0.5f;

    public float MaxEdgeModifier => maxEdgeModifier;

    #endregion

}