using System.Collections;
using System.Collections.Generic;
using Apis;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentNavTarget : MonoBehaviour , ISelectableNavTarget
{

    public UnityEvent OnSelectedEvent;
    public UnityEvent OnDeselectedEvent;
    
    public UITab_Inventory inven;
    
    public void KeyControl()
    {
        inven.KeyControl();
    }

    public void GamePadControl()
    {
        inven.GamePadControl();
    }

    public void OnNavigatedTo()
    {
        inven.OnNavigatedTo();
    }

    public void OnNavigatedFrom()
    {
        inven.OnNavigatedFrom();
    }

    public IUI_NavigationManager NavigationManager { get; set; }

    public bool IsAtBoundary(NavigationDirection direction)
    {
        return inven.IsAtBoundary(direction);
    }

    public void InitCheck()
    {
        inven.InitCheck();
        inven.SetNavigationManager(this);
    }

    public void OnSelected(bool focus)
    {
        OnSelectedEvent.Invoke();
    }

    public void OnDeselected()
    {
        OnDeselectedEvent.Invoke();
    }

    public void SetCurrentNavigatable(IUI_Navigatable newCurrent)
    {
        if (newCurrent != null && (EquipmentNavTarget)newCurrent.NavigationManager == this)
        {
            NavigationManager.SetCurrentNavigatable(this);
        }
    }
}
