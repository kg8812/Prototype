using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.Managers;
using Apis.UI;
using Apis.UI.Focus;
using Default;
using Managers;
using Save.Schema;
using UnityEngine;
using UnityEngine.UI;

public class KeySettingController : UISetting_Content
{
    private List<UI_KeySetButton> _buttons;

    #region 바인딩
    
    private enum ScrollRects
    {
        KeyScroll
    }

    private enum FocusParents
    {
        PresetBtns
    }

    #endregion

    private FocusParent _curFocus;
    private FocusParent _presetBtnsFocus;
    
    
    public override void Init()
    {
        base.Init();
        
        Bind<ScrollRect>(typeof(ScrollRects));
        Bind<FocusParent>(typeof(FocusParents));
        
        _presetBtnsFocus = Get<FocusParent>((int)FocusParents.PresetBtns);
        _presetBtnsFocus.InitCheck();
        
        if (_buttons == null)
        {
            _buttons ??= transform.GetComponentsInChildren<UI_KeySetButton>().ToList();
        }
        
        Get<ScrollRect>((int)ScrollRects.KeyScroll).UpdateFocusParentToScrollView(focusParent);
    }
    
    


    public override void ResetBySaveData()
    {
        foreach (var btn in _buttons)
        {
            btn.SetKeyImage();
        }
    }
    
    public override void KeyControl()
    {
        _curFocus?.KeyControl();
        
        if(InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left)))
        {
            if(_curFocus == focusParent)
                MoveToPresetBtns();
        }else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right)))
        {
            if(_curFocus == _presetBtnsFocus)
                MoveToKeyBtns();
        }
    }

    public override void GamePadControl()
    {
        _curFocus?.KeyControl();
        
        if(InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left)))
        {
            if(_curFocus == focusParent)
                MoveToPresetBtns();
        }else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right)))
        {
            if(_curFocus == _presetBtnsFocus)
                MoveToKeyBtns();
        }
    }

    public override void OnOpen()
    {
        MoveToKeyBtns();
    }

    #region Focus이동

    private void MoveToPresetBtns()
    {
        focusParent.canNoneFocus = true;
        focusParent.FocusReset();
        
        _presetBtnsFocus.canNoneFocus = false;
        _presetBtnsFocus.FocusReset();
        
        _curFocus = _presetBtnsFocus;
    }
    
    private void MoveToKeyBtns()
    {
        _presetBtnsFocus.canNoneFocus = true;
        _presetBtnsFocus.FocusReset();
        
        focusParent.canNoneFocus = false;
        focusParent.FocusReset();
        
        _curFocus = focusParent;
    }

    #endregion
}
