using System.Collections.Generic;

namespace Apis
{
    public class StatEventData : IEventData
    {
        private BonusStat _stat;
        public BonusStat stat => _stat ??= new BonusStat(); // 임시 추가 스탯

        public void Reset()
        {
            stat.Reset();
        }
    }
    
    public interface IStat
    {
        private static Dictionary<ActorStatType, bool> _positives;

        public static Dictionary<ActorStatType, bool> Positives => _positives ??= new Dictionary<ActorStatType, bool>
        {
            { ActorStatType.Atk, true },
            { ActorStatType.Def, false },
            { ActorStatType.AtkSpeed, true },
            { ActorStatType.MoveSpeed, true },
            { ActorStatType.MaxHp, true }
        };

        public ActorStatType Type { get; }
        public float Value { get; set; }
        public float Ratio { get; set; }
    }

    public class BasicStat : IStat
    {
        public BasicStat(ActorStatType statType)
        {
            Type = statType;
        }

        public ActorStatType Type { get; }

        public float Value { get; set; }

        public float Ratio { get; set; }
    }
}