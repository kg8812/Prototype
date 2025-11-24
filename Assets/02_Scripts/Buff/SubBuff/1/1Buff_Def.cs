
namespace Apis
{
    public class Buff_Def : Buff_Stat
    {
        public Buff_Def(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_Def;

        protected override ActorStatType StatType => ActorStatType.Def;
    }
}