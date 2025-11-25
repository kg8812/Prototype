using Apis;
using Sirenix.OdinInspector;
using System;
using EventData;
using UnityEngine;

public partial class Actor : IStatUser,IBarrierUser
{
    [TabGroup("기획쪽 수정 변수들/group1", "기본 스탯")]
    [LabelText("현재 체력")]
    [MinValue(0)]

    [SerializeField] protected float curHp; // 현재 체력   
    
    [SerializeField] protected StatManager _statManager;
    public virtual StatManager StatManager => _statManager;

    public void SetHpWithoutEvent(float hp)
    {
        curHp = hp;
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

    public bool IsInvincible => ImmunityController?.IsImmune("Invincible") ?? false;


    private BarrierCalculator _barrierCalculator;
    public BarrierCalculator BarrierCalculator => _barrierCalculator ??= new(EventManager, SubBuffManager);

    public float Barrier => BarrierCalculator?.Barrier ?? 0;

    public void AddBarrier(float amount)
    {
        BarrierCalculator?.AddBarrier(amount);
    }

    private TextShow _healText;
    protected virtual TextShow HealText => _healText ??= new HealTextShow(this);
    private TextShow _dmgText;
    protected virtual TextShow DmgText => _dmgText ??= new DmgTextShow(this);
    protected void ResetTextVariables()
    {
        HealText?.ResetVariables();
        DmgText?.ResetVariables();
    }
    
    public virtual float CurHp
    {
        get => curHp;
        set
        {
            int dmg = Mathf.RoundToInt(curHp - value);
            if(dmg < 0)
            {
                dmg = Math.Abs(dmg) + (int)curHp;
            }

            if (value > curHp)
            {
                float heal = value - curHp;
                HealText?.Show(heal,Position);
                curHp = Math.Min(curHp + heal, MaxHp);
                EventManager.ExecuteEvent(EventType.OnHpHeal,new EventParameters(this));
                return;
            }

            if (!Mathf.Approximately(dmg, 0))
            {
                DmgText?.Show(dmg,Position);
            }

            EventParameters parameters = new EventParameters(this)
            {
                hitData = new()
                {
                    dmg = dmg,dmgReceived = dmg
                }
            };
            
            ExecuteEvent(EventType.OnBeforeHpDown,parameters);

            BarrierCalculator?.Calculate(parameters);
            
            ExecuteEvent(EventType.OnBarrierChange,parameters);
            curHp -= parameters.hitData.dmg;
            
            ExecuteEvent(EventType.OnHpDown,parameters);
            if(curHp <=0)
            {
                Die();
            }
        }
    }
    
    
    public virtual float MaxHp => StatManager.GetFinalStat(ActorStatType.MaxHp);

    public virtual float Atk => StatManager.GetFinalStat(ActorStatType.Atk);

    public virtual float MoveSpeed => StatManager.GetFinalStat(ActorStatType.MoveSpeed);
    public virtual float AtkSpeed => StatManager.GetFinalStat(ActorStatType.AtkSpeed);

    public virtual float Def => StatManager.GetFinalStat(ActorStatType.Def);

    public virtual float CritProb => StatManager.GetFinalStat(ActorStatType.CritProb);
    public virtual float CritDmg => StatManager.GetFinalStat(ActorStatType.CritDmg);

    public void AddStat(ActorStatType statType, float amount, ValueType type)
    {
       StatManager.AddStat(statType,amount,type);
    }

    #region 대쉬 관련 (임시, 수치 정해지면 actormovement 내에 const로 뺄 듯)
    [TabGroup("기획쪽 수정 변수들/group1", "조작감")]
    [LabelText("대쉬 모서리 보정 최대 거리")]
    [SerializeField] float maxEdgeModifier = 0.5f;
    public float MaxEdgeModifier => maxEdgeModifier;
    #endregion
}
