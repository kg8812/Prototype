using System.Collections.Generic;
using System.Linq;
using Apis;
using Apis.Managers;
using Apis.UI;
using Apis.UI.Focus;
using Default;
using Managers;
using Save.Schema;
using UnityEngine.UI;

public class KeySettingController : UISetting_Content
{
    private List<UI_KeySetButton> _buttons;

    private FocusParent _curFocus;
    private FocusParent _presetBtnsFocus;


    public override void Init()
    {
        base.Init();

        Bind<UIAsset_Button>(typeof(Btns));
        Bind<ScrollRect>(typeof(ScrollRects));
        Bind<FocusParent>(typeof(FocusParents));

        _presetBtnsFocus = Get<FocusParent>((int)FocusParents.PresetBtns);
        _presetBtnsFocus.InitCheck();

        if (_buttons == null) _buttons ??= transform.GetComponentsInChildren<UI_KeySetButton>().ToList();

        Get<ScrollRect>((int)ScrollRects.KeyScroll).UpdateFocusParentToScrollView(focusParent);

        Get<UIAsset_Button>((int)Btns.Preset1Btn).InitCheck();
        Get<UIAsset_Button>((int)Btns.Preset1Btn).OnClick.AddListener(() =>
        {
            // Str - 10118291 : 기본 프리셋(키보드)를 적용시키겠습니까?
            SystemManager.SystemCheck(LanguageManager.Str(10118291), isOn =>
            {
                if (isOn) DataAccess.Settings.Data.ApplyPreset(1);
            });
        });
        Get<UIAsset_Button>((int)Btns.Preset2Btn).InitCheck();
        Get<UIAsset_Button>((int)Btns.Preset2Btn).OnClick.AddListener(() =>
        {
            // Str - 10118292 : 프리셋2(마우스)를 적용시키겠습니까?
            SystemManager.SystemCheck(LanguageManager.Str(10118292), isOn =>
            {
                if (isOn) DataAccess.Settings.Data.ApplyPreset(2);
            });
        });
    }


    public override void ResetBySaveData(SettingData data)
    {
        foreach (var btn in _buttons) btn.SetKeyImage();
    }

    public override void KeyControl()
    {
        _curFocus?.KeyControl();

        if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left)))
        {
            if (_curFocus == focusParent)
                MoveToPresetBtns();
        }
        else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right)))
        {
            if (_curFocus == _presetBtnsFocus)
                MoveToKeyBtns();
        }
    }

    public override void GamePadControl()
    {
        _curFocus?.KeyControl();

        if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left)))
        {
            if (_curFocus == focusParent)
                MoveToPresetBtns();
        }
        else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right)))
        {
            if (_curFocus == _presetBtnsFocus)
                MoveToKeyBtns();
        }
    }

    public override void OnOpen()
    {
        MoveToKeyBtns();
    }

    #region 바인딩

    private enum Btns
    {
        Preset1Btn,
        Preset2Btn
    }

    private enum ScrollRects
    {
        KeyScroll
    }

    private enum FocusParents
    {
        PresetBtns
    }

    #endregion

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