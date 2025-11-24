
namespace Apis
{
    public class Debuff_Def :Debuff_Stat
    {
        public Debuff_Def(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Debuff_Def;

        protected override ActorStatType StatType => ActorStatType.Def;

        
    }
}