using System.Linq;
using Apis;
using Save.Schema;
using Sirenix.Utilities;
using UnityEngine;

public class LobbyAccObj : ItemSelections
{
    public override void OnInteract()
    {
        base.OnInteract();
        var list = Drop();

        foreach (var x in list)
        {
            if (x is Acc_PickUp pickup)
            {
                pickup.OnCollect.AddListener(y =>
                {
                    list.Where(a => a != y).ForEach(b => GameManager.Item.AccPickUp.Return(b as Acc_PickUp));
                });
            }
        }
    }

    void CheckUnlock()
    {
        // _isInteractable =DataAccess.LobbyData.IsOpen(101);
        //
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
