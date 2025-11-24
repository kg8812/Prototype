namespace Apis
{
    public class ExpansionBullet : Buff_Base
    {
        public ExpansionBullet(Buff buff) : base(buff)
        {
        }

        public override SubBuffType Type => SubBuffType.ExpansionBullet;
    }
}