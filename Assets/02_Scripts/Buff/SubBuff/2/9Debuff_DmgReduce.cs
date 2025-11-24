
namespace Apis
{
    public class Debuff_DmgReduce : Debuff_Stat
    {
        public Debuff_DmgReduce(Buff buff) : base(buff)
        {
            
        }

        public override SubBuffType Type => SubBuffType.Debuff_DmgReduce;

        protected override ActorStatType StatType => ActorStatType.DmgReduce;
    }
}