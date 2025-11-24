namespace Apis
{
    public class StarEye : Debuff_base
    {
        public StarEye(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.StarEye;
    }
}