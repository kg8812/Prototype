using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoyoteVar<T>
{
    private T _CurrentVal;
    private float _coyoteTime;
    private Coroutine coroutine;

    public T Value {
        get {
            return _CurrentVal;
        }
        set {
            if(coroutine != null) GameManager.instance.StopCoroutineWrapper(coroutine);
            coroutine = null;
            _CurrentVal = value;
        }
    }

    public CoyoteVar(float coyoteTime = 0, T Val = default)
    {
        _coyoteTime = coyoteTime;
        _CurrentVal = Val;
        coroutine = null;
    }

    public void CoyoteSet(T value)
    {
        if(coroutine != null) GameManager.instance.StopCoroutineWrapper(coroutine);

        coroutine = GameManager.instance.StartCoroutineWrapper(CoyoteCoroutine(value, _coyoteTime));

    }

    IEnumerator CoyoteCoroutine(T value, float time)
    {
        yield return new WaitForSeconds(time);
        _CurrentVal = value;
        coroutine = null;
    }

    public static implicit operator T (CoyoteVar<T> v)
    {
        return v.Value;
    }
}
