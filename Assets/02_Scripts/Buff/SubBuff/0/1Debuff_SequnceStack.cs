
namespace Apis
{
    public class Debuff_SequnceStack : Debuff_base
    {
        public Debuff_SequnceStack(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_SequenceStack;
    }
}