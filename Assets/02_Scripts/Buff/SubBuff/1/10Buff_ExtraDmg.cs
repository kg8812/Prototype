
namespace Apis
{
    public class Buff_ExtraDmg : Buff_Stat
    {
        public Buff_ExtraDmg(Buff buff) : base(buff)
        {
            
        }
       
        public override SubBuffType Type => SubBuffType.Buff_ExtraDmg;

        protected override ActorStatType StatType => ActorStatType.ExtraDmg;
    }
}