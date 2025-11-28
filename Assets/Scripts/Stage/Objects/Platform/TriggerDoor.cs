using Apis.StageObj;

public class TriggerDoor : Door, TriggeredObj
{
    public void ChangeTrigger(int value)
    {
        MoveDoor(value == 1);
    }
}