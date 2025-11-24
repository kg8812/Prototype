namespace Apis
{
    public class MarkStack : Debuff_base
    {
        public MarkStack(Buff buff) : base(buff)
        {
            switch (buff.ValueType)
            {
                case ValueType.Value:
                    Stat.AddValue(ActorStatType.DmgReduce, -amount[0]);
                    break;
                case ValueType.Ratio:
                    Stat.AddRatio(ActorStatType.DmgReduce, -amount[0]);
                    break;
            }
        }

        public override SubBuffType Type => SubBuffType.MarkStack;
    }
}