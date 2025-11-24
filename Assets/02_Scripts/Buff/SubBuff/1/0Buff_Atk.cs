
namespace Apis
{
    public class Buff_Atk : Buff_Stat
    {
        public Buff_Atk(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_Atk;

        protected override ActorStatType StatType => ActorStatType.Atk;
    }
}