public class CommonMonster2MoveComponent : MonsterMoveComponent
{
    public override void MoveCCOn()
    {
        MoveOff();
    }

    public override void MoveCCOff()
    {
        MoveOn();
    }

    public override void MoveOn()
    {
        ableMove = true;
    }

    public override void MoveOff()
    {
        base.MoveOff();
        ableMove = false;
    }
}