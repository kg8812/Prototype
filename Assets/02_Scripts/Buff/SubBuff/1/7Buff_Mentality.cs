
namespace Apis
{
    public class Buff_Mentality : Buff_Stat
    {
        public Buff_Mentality(Buff effect) : base(effect)
        {

        }

        public override SubBuffType Type => SubBuffType.Buff_Mentality;

        protected override ActorStatType StatType => ActorStatType.Mental;
    }
}