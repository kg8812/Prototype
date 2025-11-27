using System.Collections;
using UnityEngine;

public class CoyoteVar<T>
{
    private readonly float _coyoteTime;
    private T _CurrentVal;
    private Coroutine coroutine;

    public CoyoteVar(float coyoteTime = 0, T Val = default)
    {
        _coyoteTime = coyoteTime;
        _CurrentVal = Val;
        coroutine = null;
    }

    public T Value
    {
        get => _CurrentVal;
        set
        {
            if (coroutine != null) GameManager.instance.StopCoroutineWrapper(coroutine);
            coroutine = null;
            _CurrentVal = value;
        }
    }

    public void CoyoteSet(T value)
    {
        if (coroutine != null) GameManager.instance.StopCoroutineWrapper(coroutine);

        coroutine = GameManager.instance.StartCoroutineWrapper(CoyoteCoroutine(value, _coyoteTime));
    }

    private IEnumerator CoyoteCoroutine(T value, float time)
    {
        yield return new WaitForSeconds(time);
        _CurrentVal = value;
        coroutine = null;
    }

    public static implicit operator T(CoyoteVar<T> v)
    {
        return v.Value;
    }
}