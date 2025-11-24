namespace Apis
{
    public class Buff_Warmth : Buff_Base
    {
        public Buff_Warmth(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_Warmth;
    }
}