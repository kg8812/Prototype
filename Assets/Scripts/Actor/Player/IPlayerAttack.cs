public interface IPlayerAttack
{
    public float GroundAttackEscapeTime(int index);
    public float AirAttackEscapeTime(int index);
    void Attack();
    bool CheckAttackable(int index);

    public void Attack(int combo);
}

public class PlayerBasicAttack : IPlayerAttack
{
    private readonly Player player;

    public PlayerBasicAttack(Player player)
    {
        this.player = player;
    }

    float IPlayerAttack.GroundAttackEscapeTime(int index)
    {
        return 0;
    }

    float IPlayerAttack.AirAttackEscapeTime(int index)
    {
        return 0;
    }

    public void Attack()
    {
    }

    public bool CheckAttackable(int index)
    {
        return true;
    }

    public void Attack(int combo)
    {
    }
}
