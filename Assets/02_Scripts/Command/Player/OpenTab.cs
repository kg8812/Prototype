using Command;
using Apis;
using GameStateSpace;
using UnityEngine;

[CreateAssetMenu(fileName = "Tab", menuName = "ActorCommand/Player/Tab")]
public class OpenTab : PlayerCommand
{
    protected override void Invoke(Player go)
    {
        GameManager.UI.CreateUI("UI_TabMenu", UIType.Scene);
    }

    public override bool InvokeCondition(Player go)
    {
        return true;
    }
}