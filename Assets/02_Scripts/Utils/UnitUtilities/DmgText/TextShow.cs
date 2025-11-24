using UnityEngine;

public abstract class TextShow
{
    protected bool _isShowing;
    protected float _textAmount;

    public abstract void Show(float amount, Vector2 pos);
    public abstract void ResetVariables();
}
