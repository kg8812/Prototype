using Save.Schema;

public class WellOfWarmth : SingleUseInteraction
{

    bool Check()
    {
        return DataAccess.LobbyData.IsOpen(401);
    }
    private void Start()
    {
        InteractCheckEvent += Check;
    }

    public override void OnInteract()
    {
        if (GameManager.instance.Player.CurrentPotionCapacity < GameManager.instance.Player.MaxPotionCapacity)
        {
            GameManager.instance.Player.increasePotionCapacity(GameManager.Save.currentSlotData.GrowthSaveData.ObjectData.wellOfWarmthCount);
            base.OnInteract();
        }
    }
}
