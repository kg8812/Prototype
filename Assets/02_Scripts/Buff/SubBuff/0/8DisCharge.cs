namespace Apis
{
    public class DisCharge : Debuff_base
    {
        public DisCharge(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.DisCharge;
    }
}