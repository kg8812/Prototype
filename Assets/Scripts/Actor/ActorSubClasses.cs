using System;
using System.Collections.Generic;
using Default;
using EventData;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Apis
{
    public class ActorEvents
    {
        private readonly BuffEvent _buffEvent;
        private readonly CollisionEventHandler _collisionEventHandler;

        private List<IEventChild> _eventChildren;

        public ActorEvents(GameObject owner)
        {
            _buffEvent = owner.GetOrAddComponent<BuffEvent>();
            _collisionEventHandler = owner.GetOrAddComponent<CollisionEventHandler>();
            _eventChildren = new List<IEventChild>
            {
                _buffEvent, _collisionEventHandler
            };
        }

        public IEventManager EventManager => _buffEvent;
        public List<IEventChild> EventChildren => _eventChildren;

        public void AddEvent(EventType eventType, UnityAction<EventParameters> action)
        {
            EventManager?.AddEvent(eventType, action);
        }

        public void RemoveEvent(EventType eventType, UnityAction<EventParameters> action)
        {
            EventManager?.RemoveEvent(eventType, action);
        }

        public void ExecuteEvent(EventType eventType, EventParameters parameters)
        {
            EventManager?.ExecuteEvent(eventType, parameters);
        }
    }

    public class ActorHpController
    {
        private readonly Actor _actor;
        private readonly BarrierCalculator _barrierCalculator;

        private TextShow _dmgText;
        private TextShow _healText;

        private float _curHp;

        public ActorHpController(Actor actor)
        {
            _actor = actor;
            _barrierCalculator = new BarrierCalculator(_actor.EventManager);
        }

        private TextShow DmgText => _dmgText ??= new DmgTextShow(_actor);
        private TextShow HealText => _healText ??= new HealTextShow(_actor);

        public float CurHp => _curHp;
        public float MaxHp => _actor.StatManager.GetFinalStat(ActorStatType.MaxHp);
        public float Barrier => _barrierCalculator?.Barrier ?? 0f;
        public BarrierCalculator BarrierCalculator => _barrierCalculator;

        public void SetHpWithoutEvent(float hp)
        {
            _curHp = hp;
        }

        public float ApplyHeal(float value)
        {
            if (value <= 0) return 0;
            if (_curHp >= MaxHp) return 0;

            var heal = Mathf.Min(value, MaxHp - _curHp);
            HealText?.Show(heal, _actor.Position);
            _curHp += heal;
            _actor.EventManager.ExecuteEvent(EventType.OnHpHeal, new EventParameters(_actor));

            return heal;
        }

        public float ApplyDmgToTargetValue(float value)
        {
            var dmg = Mathf.RoundToInt(_curHp - value);
            if (dmg < 0) dmg = Math.Abs(dmg) + (int)_curHp;

            if (value > _curHp)
            {
                var heal = value - _curHp;
                return ApplyHeal(heal);
            }
            
            var parameters = new EventParameters(_actor);
            parameters.Set(new HitEventData()
            {
                dmg = dmg, dmgReceived = dmg
            });

            return ApplyDamage(parameters);
        }

        public float ApplyDamage(EventParameters parameters)
        {
            if (parameters == null) return 0;
            var hitData = parameters.Get<HitEventData>();
            if (hitData == null) return 0;

            _actor.ExecuteEvent(EventType.OnBeforeHpDown, parameters);

            BarrierCalculator?.Calculate(parameters);
            _actor.ExecuteEvent(EventType.OnBarrierChange, parameters);

            var finalDmg = Mathf.Max(0, hitData.dmg);

            if (!Mathf.Approximately(finalDmg, 0))
                DmgText?.Show(finalDmg, _actor.Position);

            _curHp -= finalDmg;

            _actor.ExecuteEvent(EventType.OnHpDown, parameters);
            if (_curHp <= 0) _actor.Die();

            return finalDmg;
        }

        public void AddBarrier(float amount)
        {
            _barrierCalculator?.AddBarrier(amount);
        }

        public void ResetTextVariables()
        {
            HealText?.ResetVariables();
            DmgText?.ResetVariables();
        }
    }

    public class ActorCombat
    {
        private readonly Actor _actor;
        private Guid recentHitInfo; // 최근에 피격당한 공격 혹은 스킬 (중복 체크용)

        public ActorCombat(Actor actor)
        {
            _actor = actor;
        }
        
        public EventParameters Attack(EventParameters eventParameters)
        {
            if (_actor?.gameObject == null || _actor.IsDead) return eventParameters;

            if (eventParameters?.target == null || eventParameters.target.IsInvincible) return eventParameters;

            BonusStat Action()
            {
                return eventParameters.Get<StatEventData>().stat;
            }

            _actor.BonusStatEvent += Action;

            try
            {
                // 이벤트 실행을 데미지 계산 전에 호출해야함
                // 데미지 증가, 크리티컬 확률 증가 등 효과들이 적용되어야 하기 때문
                if (eventParameters.target is Actor)
                {
                    // 타격 성공 판정도 Actor 한정으로만 (다른 IOnHit는 불가)
                    _actor.ExecuteEvent(EventType.OnAttackSuccess, eventParameters);
                    if (eventParameters.Get<AttackEventData>().attackType == Define.AttackType.BasicAttack)
                        _actor.ExecuteEvent(EventType.OnBasicAttack, eventParameters);
                }

                eventParameters.Get<AttackEventData>().dmg =
                    eventParameters.Get<AttackEventData>().atkStrategy.Calculate(eventParameters.target);
                ApplyCritical(eventParameters);
                ApplyBackAttack(eventParameters);

                eventParameters.Get<HitEventData>().dmg = eventParameters.Get<AttackEventData>().dmg;

                eventParameters.Get<HitEventData>().dmgReceived = eventParameters.target.OnHit(eventParameters);

                _actor.ExecuteEvent(EventType.OnAfterAtk, eventParameters);
            }
            finally
            {
                _actor.BonusStatEvent -= Action;
            }

            return eventParameters;
        }
        
        void ApplyCritical(EventParameters eventParameters)
        {
            var random = Random.Range(0, 100f);
            var prob = _actor.CritProb;

            if (random < prob || eventParameters.Get<AttackEventData>().isfixedCrit)
            {
                eventParameters.Get<AttackEventData>().dmg *= _actor.CritDmg * 0.01f;

                _actor.ExecuteEvent(EventType.OnCrit, eventParameters);
                eventParameters.Get<HitEventData>().isCritApplied = true;
            }
            else
            {
                eventParameters.Get<HitEventData>().isCritApplied = false;
            }
        }

        void ApplyBackAttack(EventParameters eventParameters)
        {
            if (eventParameters.target is not Actor actor)
                return;
            
            if (Vector2.Dot(actor.transform.right * (int)actor.Direction,
                    (_actor.transform.position - eventParameters.target.gameObject.transform.position).normalized) < 0)
                _actor.ExecuteEvent(EventType.OnBackAttack, eventParameters);
        }
        
        public virtual float ReceiveHit(EventParameters parameters)
        {
            if (_actor.IsInvincible || parameters == null || _actor.IsDead) return 0;

            BonusStat Action()
            {
                return parameters.Get<StatEventData>().stat;
            }
            _actor.BonusStatEvent += Action;

            try
            {
                _actor.ExecuteEvent(EventType.OnBeforeHit, parameters);

                if (parameters.Get<HitEventData>().hitDisable) return 0;

                ApplyDef(parameters);

                _actor.ExecuteEvent(EventType.OnHit, parameters);
                if (parameters.Get<HitEventData>().isCritApplied) _actor.ExecuteEvent(EventType.OnCritHit, parameters);
                _actor.CurHp -= parameters.Get<HitEventData>().dmg;

                _actor.ExecuteEvent(EventType.OnAfterHit, parameters); 
                
                recentHitInfo = parameters.Get<AttackEventData>().attackGuid;
                
                return parameters.Get<HitEventData>().dmg;
            }
            finally
            { 
                _actor.BonusStatEvent -= Action;
            }
        }
        
        void ApplyDef(EventParameters eventParameters)
        {
            eventParameters.Get<HitEventData>().dmg *=
                1 - (1 - FormulaConfig.defConstant / (FormulaConfig.defConstant + _actor.Def));
        }
        
        public bool CheckDuplicationAtk(AttackObject atkObj)
        {
            return recentHitInfo != Guid.Empty && atkObj.firedAtkGuid == recentHitInfo;
        }
    }

    public class ActorImmunity
    {
        public const string Invincible = "Invincible";
        public const string HitImmunity = "HitImmunity";
        
        public readonly ImmunityController Controller = new();
        
        public bool IsInvincible => Controller.IsImmune(Invincible);
        public bool IsHitImmune => Controller.IsImmune(HitImmunity);

        public Guid AddInvincible() => Add(Invincible);
        public void RemoveInvincible(Guid guid) => Remove(Invincible, guid);

        public Guid AddHitImmunity() => Add(HitImmunity);
        public void RemoveHitImmunity(Guid guid) => Remove(HitImmunity, guid);

        public void ClearHitImmunity() => Controller.MakeCountToZero(HitImmunity);

        private Guid Add(string type)
        {
            if (!Controller.Contains(type))
                Controller.MakeNewType(type);

            return Controller.AddCount(type);
        }

        private void Remove(string type, Guid guid)
        {
            Controller.MinusCount(type, guid);
        }
    }
}