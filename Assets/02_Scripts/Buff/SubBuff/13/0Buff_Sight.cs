namespace Apis
{
    public class Buff_Sight : SubBuff
    {
        public Buff_Sight(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_Sight;
    }
}