using System;
using System.Collections.Generic;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Apis
{
    [FoldoutGroup("기획쪽 수정 변수들")]
    [TabGroup("기획쪽 수정 변수들/group1", "기본 스탯")]
    [Serializable]
    [HideLabel]
    public class StatManager
    {
        public delegate BonusStat StatEvent();

        [SerializeField] protected BaseStat _baseStat;


        private BonusStat _bonusStat;

        private Dictionary<ActorStatType, IStatCalculator> _statStrategies;
        private BonusStat _temp;

        public StatManager()
        {
        }

        public StatManager(StatManager other)
        {
            _baseStat = new BaseStat(other._baseStat); 
            _bonusStat = other._bonusStat != null ? new BonusStat(other._bonusStat) : null;

        }

        public virtual BaseStat BaseStat
        {
            get => _baseStat;
            set => _baseStat = value;
        }

        public BonusStat BonusStat
        {
            get
            {
                _bonusStat ??= new BonusStat();

                _temp ??= new BonusStat();
                _temp.Reset();
                _temp += _bonusStat;
                if (bonusStatEvent != null)
                    foreach (var ev in bonusStatEvent.GetInvocationList())
                        if (ev is StatEvent st)
                            _temp += st();

                return _temp;
            }
        }

        public Dictionary<ActorStatType, IStatCalculator> StatStrategies
        {
            get
            {
                if (_statStrategies == null)
                {
                    _statStrategies = new Dictionary<ActorStatType, IStatCalculator>();
                    foreach (var x in Utils.StatTypes) _statStrategies.Add(x, new BasicStatStrategy(this));
                }

                return _statStrategies;
            }
        }

        private event StatEvent bonusStatEvent;

        public event StatEvent BonusStatEvent
        {
            add
            {
                bonusStatEvent -= value;
                bonusStatEvent += value;
            }
            remove => bonusStatEvent -= value;
        }

        public void AddStat(ActorStatType statType, float amount, ValueType type)
        {
            switch (type)
            {
                case ValueType.Value:
                    _bonusStat.AddValue(statType, amount);
                    break;
                case ValueType.Ratio:
                    _bonusStat.AddRatio(statType, amount);
                    break;
            }
        }

        public float GetFinalStat(ActorStatType statType)
        {
            return StatStrategies[statType].GetFinalStat(statType);
        }
    }

    public class PlayerStatManager : StatManager
    {
        public PlayerStatManager(StatManager other) : base(other)
        {
        }

        public override BaseStat BaseStat => _baseStat;
    }
}