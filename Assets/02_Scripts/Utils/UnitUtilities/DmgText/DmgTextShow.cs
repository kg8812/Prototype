using System;
using System.Collections;
using Apis;
using UnityEngine;

public class DmgTextShow : TextShow
{
    bool _isCrit;

    public DmgTextShow()
    {
        
    }
    public DmgTextShow(IEventUser user)
    {
        user?.EventManager.AddEvent(EventType.OnCritHit, _ =>
        {
            _isCrit = true;
        });
    }

    public override void Show(float amount, Vector2 pos)
    {
        if (!_isShowing)
        {
            GameManager.instance.StartCoroutine(ShowDmgInTime(pos));
        }
        _textAmount += amount;
    }

    public override void ResetVariables()
    {
        _isShowing = false;
        _textAmount = 0;
        _isCrit = false;
    }
    
    IEnumerator ShowDmgInTime(Vector2 Position)
    {
        if (!_isShowing)
        {
            _isShowing = true;
            yield return new WaitForEndOfFrame();

            
            DmgTextManager.ShowDmgText(Position, Math.Abs(_textAmount), _isCrit ? Color.red : Color.white);

            _textAmount = 0;
            _isShowing = false;
            _isCrit = false;
        }
    }
    
}
