
namespace Apis
{
    public abstract class Buff_Stat : Buff_Base
    {
        protected Buff_Stat(Buff buff) : base(buff)
        {
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    Stat.AddValue(StatType, amount[0]);
                    break;
                case ValueType.Ratio:
                    Stat.AddRatio(StatType, amount[0]);
                    break;
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            actor.AddStat(StatType, amount[0], buff.ValueType);
        }
        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    parameters.statData.stat.AddValue(StatType, amount[0]);
                    break;
                case ValueType.Ratio:
                    parameters.statData.stat.AddRatio(StatType, amount[0]);
                    break;
            }
        }

       
        protected abstract ActorStatType StatType { get; }
    }
}