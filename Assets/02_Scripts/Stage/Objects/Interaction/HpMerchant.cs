using System;
using Apis;
using Apis;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
[RequireComponent(typeof(Interaction))]
public class HpMerchant : MonoBehaviour,IOnInteract
{
    public Func<bool> InteractCheckEvent { get; set; }

    ItemDropper _dropper;
    public int hpCost;

    private void Awake()
    {
        _dropper = GetComponent<ItemDropper>();
    }

    public void OnInteract()
    {
        if (GameManager.instance.Player.CurHp <= hpCost) return;

        GameManager.instance.Player.CurHp -= hpCost;
        _dropper.Drop();
    }
}
