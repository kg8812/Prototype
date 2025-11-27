using Apis;

public enum AttackCategory
{
    One,
    Two,
    Three,
    Four,
    Five
}

public interface IAttackItem
{
    public AttackCategory Category { get; }

    public UI_AtkItemIcon Icon { get; set; }

    public int AtkSlotIndex { get; set; }

    // public string Name { get; }
    public void BeforeAttack();
    public void UseAttack();
    public bool TryAttack();
    public void Equip(IMonoBehaviour user);
    public void UnEquip();
    void WhenIconIsSet(UI_AtkItemIcon icon);
    public void EndAttack();

    public void SetIcon(UI_AtkItemIcon icon)
    {
        Icon = icon;
        WhenIconIsSet(icon);
    }
}

public interface IAttackItemStat
{
    public float Atk { get; }
}