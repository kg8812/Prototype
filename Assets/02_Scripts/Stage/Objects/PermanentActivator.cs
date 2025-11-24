using System;
using System.Collections;
using System.Collections.Generic;
using Save.Schema;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class PermanentActivator : SerializedMonoBehaviour
{
    public UnityEvent ActivateEvent;

    public Func<bool> CheckCondition;

    private void Start()
    {
        if (CheckCondition == null || CheckCondition())
        {
            ActivateEvent?.Invoke();
        }
    }
}
