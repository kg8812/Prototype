namespace Apis
{
    public class Celerity : Buff_Base
    {
        public Celerity(Buff buff) : base(buff)
        {
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    Stat.AddValue(ActorStatType.Atk,amount[0]);
                    Stat.AddValue(ActorStatType.MoveSpeed,amount[0]);
                    break;
                case ValueType.Ratio: 
                    Stat.AddRatio(ActorStatType.Atk,amount[0]);
                    Stat.AddRatio(ActorStatType.MoveSpeed,amount[0]);
                    break;
            }
        }

        public override void TempApply(EventParameters parameters)
        {
            base.TempApply(parameters);
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    parameters.statData.stat.AddValue(ActorStatType.Atk,amount[0]);
                    parameters.statData.stat.AddValue(ActorStatType.MoveSpeed,amount[0]);
                    break;
                case ValueType.Ratio: 
                    parameters.statData.stat.AddRatio(ActorStatType.Atk,amount[0]);
                    parameters.statData.stat.AddRatio(ActorStatType.MoveSpeed,amount[0]);
                    break;
            }
        }

        public override void PermanentApply()
        {
            base.PermanentApply();
            
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    actor.AddStat(ActorStatType.Atk,amount[0],ValueType.Value);
                    actor.AddStat(ActorStatType.Atk,amount[0],ValueType.Value);
                    break;
                case ValueType.Ratio: 
                    actor.AddStat(ActorStatType.Atk,amount[0],ValueType.Ratio);
                    actor.AddStat(ActorStatType.Atk,amount[0],ValueType.Ratio);
                    break;
            }
        }

        public override SubBuffType Type => SubBuffType.Celerity;
    }
}