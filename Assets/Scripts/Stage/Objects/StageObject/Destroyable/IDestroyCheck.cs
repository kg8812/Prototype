using EventData;

public interface IDestroyCheck
{
    public bool CheckDestroyable(EventParameters parameters);
}

public class AttackTypeCheck : IDestroyCheck
{
    private readonly Define.AttackType _attackType;

    public AttackTypeCheck(Define.AttackType attackType)
    {
        _attackType = attackType;
    }

    public bool CheckDestroyable(EventParameters parameters)
    {
        return parameters?.Get<AttackEventData>() != null && parameters.Get<AttackEventData>().attackType == _attackType;
    }
}