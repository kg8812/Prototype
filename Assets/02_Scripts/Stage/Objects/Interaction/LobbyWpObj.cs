using System.Linq;
using Apis;
using Save.Schema;
using Sirenix.Utilities;

public class LobbyWpObj : ItemSelections
{
    public override void OnInteract()
    {
        base.OnInteract();
        var list = Drop();

        foreach (var x in list)
        {
            if (x is Weapon_PickUp pickup)
            {
                pickup.OnCollect.AddListener(y =>
                {
                    list.Where(a => a != y).ForEach(b => GameManager.Item.WeaponPickUp.Return(b as Weapon_PickUp));
                });
            }
        }
    }
    void CheckUnlock()
    {
        // _isInteractable = DataAccess.LobbyData.IsOpen(301);
        // if (InteractCheckEvent)
        // {
        //     GameManager.instance.WhenUnlock.RemoveListener(CheckUnlock);
        // }
    }
    private void Start()
    {
        CheckUnlock();
        GameManager.instance.WhenUnlock.RemoveListener(CheckUnlock);
        GameManager.instance.WhenUnlock.AddListener(CheckUnlock);
    }
}
