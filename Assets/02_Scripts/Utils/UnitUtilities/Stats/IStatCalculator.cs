using UnityEngine;

namespace Apis
{
    public interface IStatCalculator
    {
        public float GetFinalStat(ActorStatType statType);
    }

    public class BasicStatStrategy : IStatCalculator
    {
        readonly StatManager _statManager;

        public BasicStatStrategy(StatManager statManager)
        {
            _statManager = statManager;
        }

        public float GetFinalStat(ActorStatType statType)
        {
            if (_statManager == null) return 0;

            float value =
                ((_statManager.BaseStat?.Get(statType) ?? 0) + (_statManager.BonusStat?.Stats[statType].Value ?? 0)) *
                (1 + (_statManager.BonusStat?.Stats[statType].Ratio ?? 0) / 100f);
            return IStat.Positives[statType] && value < 0 ? 0 : value;
        }
    }
}