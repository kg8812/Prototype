using Apis;
using Apis.DataType;
using Sirenix.OdinInspector;

public class HotOrange : CollideItem
{
    [LabelText("지속시간")] public float duration;
    [LabelText("방어력 증가량")]public float amount;
    [LabelText("버프 최대스택")] public int maxStack;
    [LabelText("잔재 획득량")] public int goldAmount;

    private Buff buff;

    public void Init(HotOrangeTree.OrangeInfo info)
    {
        duration = info.duration;
        amount = info.amount;
        maxStack = info.maxStack;
        goldAmount = info.goldAmount;
    }
    public override void InvokeInteraction()
    {
        if (buff == null)
        {
            BuffDataType data = new(SubBuffType.Buff_Def)
            {
                buffPower = new []{amount}, buffCategory = 1, buffDuration = duration,
                buffDispellType = 1,
                buffMaxStack = maxStack, valueType = ValueType.Value, showIcon = false
            };
            buff = new(data, GameManager.instance.Player);
        }
        
        buff.AddSubBuff(GameManager.instance.Player,new EventParameters(GameManager.instance.Player));
        GameManager.instance.Soul += goldAmount;
    }
}
