using System.Collections.Generic;

namespace Apis
{
    public interface IStat
    {
        private static Dictionary<ActorStatType, bool> _positives;
        public static Dictionary<ActorStatType, bool> Positives => _positives ??= new()
        {
            { ActorStatType.Atk, true },
            { ActorStatType.Def, false },
            { ActorStatType.AtkSpeed, true },
            { ActorStatType.MoveSpeed, true },
            { ActorStatType.CritProb, true },
            { ActorStatType.CritDmg, true },
            { ActorStatType.CDReduction, false },
            { ActorStatType.Mental, false },
            { ActorStatType.MaxHp, true },
            { ActorStatType.DmgReduce, false },
            { ActorStatType.ExtraDmg, false },
            { ActorStatType.GoldRate, false },
            { ActorStatType.HealRate, false },
            { ActorStatType.ShieldRate, false },
            { ActorStatType.CritHit, false },
            { ActorStatType.Body, true },
            { ActorStatType.Spirit, true },
            { ActorStatType.Finesse, true },
            { ActorStatType.CastingSpeed ,false},
            { ActorStatType.CastingDmg ,false}
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