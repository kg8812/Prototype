using System;
using Apis;
using Default;
using UnityEngine;
using UnityEngine.Events;

public class UIObject1 : MonoBehaviour,IOnInteract
{
    protected UI_Base ui;
    [SerializeField] string uiName;
    [SerializeField] UIType uiType;

    public bool isOnceUse = false;

    private bool isUsed;

    bool Check()
    {
        return !isOnceUse || (isOnceUse && !isUsed);
    }
    private void Start()
    {
        InteractCheckEvent += Check;
    }

    public Func<bool> InteractCheckEvent { get; set; }

    public virtual void OnInteract()
    {
        ui = GameManager.UI.CreateUI(uiName, uiType);
        isUsed = true;
    }
}
