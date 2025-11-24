using System;
using UnityEngine;

public class Scent : MonoBehaviour, IOnInteract
{
    public Func<bool> InteractCheckEvent { get; set; }

    private bool isUsed;
    
    bool Check()
    {
        return !isUsed;
    }
    private void Awake()
    {
        isUsed = false;
        InteractCheckEvent += Check;
    }
    public void OnInteract()
    {
        isUsed = true;
    }
}
