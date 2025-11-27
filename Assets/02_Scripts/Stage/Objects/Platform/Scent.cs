using System;
using UnityEngine;

public class Scent : MonoBehaviour, IOnInteract
{
    private bool isUsed;

    private void Awake()
    {
        isUsed = false;
        InteractCheckEvent += Check;
    }

    public Func<bool> InteractCheckEvent { get; set; }

    public void OnInteract()
    {
        isUsed = true;
    }

    private bool Check()
    {
        return !isUsed;
    }
}