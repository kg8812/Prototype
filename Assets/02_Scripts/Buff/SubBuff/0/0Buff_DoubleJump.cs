
namespace Apis
{
    public class Buff_DoubleJump : Buff_Base
    {
        public Buff_DoubleJump(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.Buff_DoubleJump;       

        public override void OnAdd()
        {
            base.OnAdd();
            actor.GetComponent<Player>().playerStat.JumpMax++;
        }

        public override void OnRemove()
        {
            base.OnRemove();

            actor.GetComponent<Player>().playerStat.JumpMax--;
        }
        public override void PermanentApply()
        {
            base.PermanentApply();
            actor.GetComponent<Player>().playerStat.JumpMax++;
        }
       
    }
}