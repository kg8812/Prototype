namespace Apis.CommonMonster2
{
    public partial class CommonMonster2
    {
        public override void EndStun()
        {
            base.EndStun();
            if (!IsDead)
                IdleOn();
        }
    }
}