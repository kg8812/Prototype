using System;
using System.Collections;
using Apis;
using UnityEngine;

public class HealTextShow : TextShow
{
    private IEventUser user;

    public HealTextShow(IEventUser user)
    {
        this.user = user;
    }

    private IEnumerator ShowHealInTime(Vector2 Position)
    {
        if (!_isShowing)
        {
            _isShowing = true;
            yield return new WaitForEndOfFrame();

            if (_textAmount > 0) DmgTextManager.ShowDmgText(Position, Math.Abs(_textAmount), Color.green);

            _textAmount = 0;
            _isShowing = false;
        }
    }

    public override void Show(float amount, Vector2 pos)
    {
        if (!_isShowing) GameManager.instance.StartCoroutine(ShowHealInTime(pos));

        _textAmount += amount;
    }

    public override void ResetVariables()
    {
        _textAmount = 0;
        _isShowing = false;
    }
}