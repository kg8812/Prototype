namespace Apis
{
    public class Buff_AllStat : Buff_Base
    {
        public Buff_AllStat(Buff buff) : base(buff)
        {
            foreach (var actorStatType in Default.Utils.StatTypes)
            {
                switch (buff.ValueType)
                {
                    case ValueType.Value:
                        Stat.AddValue(actorStatType,amount[0]);
                        break;
                    case ValueType.Ratio:
                        Stat.AddRatio(actorStatType,amount[0]);
                        break;
                }
            }
        }

        public override SubBuffType Type => SubBuffType.Buff_AllStat;
    }
}