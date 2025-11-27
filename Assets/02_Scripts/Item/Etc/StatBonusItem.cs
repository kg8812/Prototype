using UnityEngine;

public class StatBonusItem : EtcItem
{
    [SerializeField] private string _name;
    protected string flavourText;
    protected string description;

    [SerializeField] private int itemId = 8401;
    // public override string Name => _name;
    // public override string FlavourText => flavourText;
    //
    // public override string Description => description;

    public ActorStatType statType;
    public int amount;

    public override int ItemId => itemId;

    public override void Init()
    {
        base.Init();
    }

    public override void Activate()
    {
    }

    protected override void OnCollect()
    {
        base.OnCollect();
        GameManager.instance.Player.AddStat(statType, amount, ValueType.Value);
    }

    protected override void OnRemove()
    {
        base.OnRemove();
        GameManager.instance.Player.AddStat(statType, -amount, ValueType.Value);
    }
}