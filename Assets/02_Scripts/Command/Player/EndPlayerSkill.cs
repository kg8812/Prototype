using Command;
using UnityEngine;

[CreateAssetMenu(fileName = "EndActiveSkill", menuName = "ActorCommand/Player/EndActiveSkill")]

public class EndPlayerSkill : PlayerCommand
{
    protected override void Invoke(Player go)
    {
        go?.ActiveSkill?.DeActive();
    }

    public override bool InvokeCondition(Player go)
    {
        return true;
    }
}
