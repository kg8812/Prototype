using System;
using Apis;
using UnityEngine;

public class UIObject2 : MonoBehaviour,IOnInteract
{
    [SerializeField] string uiCanvasName;

    public Func<bool> InteractCheckEvent { get; set; }

    public void OnInteract()
    {
        GameManager.UI.CreateUI(uiCanvasName, UIType.Scene);
    }
}
