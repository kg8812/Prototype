namespace Apis
{
    public partial class Monster
    {
        public void MoveOn()
        {
            MoveComponent.MoveOn();
        }

        public void MoveOff()
        {
            MoveComponent.MoveOff();
        }

        public void MoveCCOn()
        {
            MoveComponent.MoveCCOn();
        }

        public void MoveCCOff()
        {
            MoveComponent.MoveCCOff();
        }

        public void JumpOn()
        {
            MoveComponent.JumpOn();
        }

        public void JumpOff()
        {
            MoveComponent.JumpOff();
        }

        public override void AttackOn()
        {
            ableAttack = true;
        }

        public override void AttackOff()
        {
            ableAttack = false;
        }
    }
}