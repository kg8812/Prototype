using System.Collections;
using System.Collections.Generic;
using Save.Schema;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public static class KeySettingManager
{
    public static KeyCode GetGameKeyCode(Define.GameKey gameKey)
    {
        return DataAccess.Settings.Data.gameKeys.GetValueOrDefault(gameKey, KeyCode.None);
    }

    public static ButtonControl GetGameButton(Define.GameKey gameKey)
    {
        return DataAccess.Settings.Data.GamePadKeys.GetValueOrDefault(gameKey, null);
    }

    public static KeyCode GetUIKeyCode(Define.UIKey uiKey)
    {
        return DataAccess.Settings.Data.UIKeys.GetValueOrDefault(uiKey, KeyCode.None);
    }

    public static ButtonControl GetUIButton(Define.UIKey uiKey)
    {
        return DataAccess.Settings.Data.UIButtons.GetValueOrDefault(uiKey, null);
    }
}
