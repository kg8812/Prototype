using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectDestroyer : MonoBehaviour
{
    private bool isReturn;

    private void OnEnable()
    {
        isReturn = false;
    }

    IEnumerator Destroy(float time,UnityAction AfterDestroy)
    {
        if (isReturn) yield break;
        isReturn = true;
        yield return new WaitForSeconds(time);
        GameManager.Factory.Return((gameObject));
        isReturn = false;
        AfterDestroy.Invoke();
    }
    public void DestroyInTime(float time,UnityAction AfterDestroy)
    {
        if (!gameObject.activeSelf) return;
        StartCoroutine(Destroy(time,AfterDestroy));
    }
}
