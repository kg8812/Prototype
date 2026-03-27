using System;
using Apis;
using Default;
using UnityEngine;

public class UIObject1 : MonoBehaviour, IOnInteract
{
    [SerializeField] private string uiName;
    [SerializeField] private UIType uiType;

    public bool isOnceUse;

    private bool isUsed;
    protected UI_Base ui;

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

    private bool Check()
    {
        return !isOnceUse || (isOnceUse && !isUsed);
    }
}