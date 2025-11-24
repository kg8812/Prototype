using System.Collections.Generic;
using Apis;
using Save.Schema;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public interface IController
{
    public void KeyControl();
    public void GamePadControl();
}


public class DefaultController : IController
{
    public void KeyControl()
    {
        if (InputManager.GetKeyDown(KeySettingManager.GetGameKeyCode(Define.GameKey.Escape)) && !GameManager.instance.playerDied)
        {
            // 이거만 예외처리.
            GameManager.UI.CreateUI("UI_MenuPopup", UIType.Popup);
        }
    }

    public void GamePadControl()
    {
        if (InputManager.GetButtonDown(KeySettingManager.GetGameButton(Define.GameKey.Escape))&& !GameManager.instance.playerDied)
        {
            GameManager.UI.CreateUI("UI_MenuPopup", UIType.Popup);
        }
    }
}

public class InputManager
{
    public static List<KeyCode> pushDownedKeyInFrame = new List<KeyCode>();
    public static List<KeyCode> pushedKeyInFrame = new();
    public static List<KeyCode> pushUpedKeyInFrame = new();

    public static List<ButtonControl> pushedDownedButtonInFrame = new();
    public static List<ButtonControl> pushedButtonInFrame = new();
    public static List<ButtonControl> pushedUpButtonInFrame = new();

    /**
     * 한 프레임에 여러개의 컨트롤러를 동작시키지만 두개의 컨트롤러에서 같은 키코드를 받을 때, 둘 다 발생하는 문제가 생김
     * 따라서 모든 키 입력을 해당 함수로 진행하고, 해당 프레임에서 입력이 되어있으면 다음 거는 입력받지 않도록 처리.
     * 하지만 이를 적용시 발생할 수 있는 문제점이, 우선순위에 있는 컨트롤러에서 특정 키를 눌러 로직을 실행할려고 했는데 조건이 맞지 않아 탈출.
     * 하지만 pushedKeyInFrame에 키코드가 추가되어있어, 다음 컨트롤러에서 입력을 안받음.
     * GetKeyDown을 되도록이면 가장 마지막에 판단해줬으면..(예외사항이 esc밖에 없긴함)
     */

    public static void ClearPushedButtons()
    {
        pushedDownedButtonInFrame.Clear();
        pushedButtonInFrame.Clear();
        pushedUpButtonInFrame.Clear();
    }
    public static void ClearPushedKeycode()
    {
        pushDownedKeyInFrame.Clear();
        pushedKeyInFrame.Clear();
        pushUpedKeyInFrame.Clear();
    }

    public static bool GetButtonDown(ButtonControl button)
    {
        if (pushedDownedButtonInFrame.Contains(button))
        {
            return false;
        }

        if (button.wasPressedThisFrame)
        {
            pushedDownedButtonInFrame.Add(button);
            return true;
        }

        return false;
    }
     public static bool GetButton(ButtonControl button)
    {
        if (pushedButtonInFrame.Contains(button))
        {
            return false;
        }

        if (button.isPressed)
        {
            pushedButtonInFrame.Add(button);
            return true;
        }

        return false;
    }
     public static bool GetButtonUp(ButtonControl button)
    {
        if (pushedUpButtonInFrame.Contains(button))
        {
            return false;
        }

        if (button.wasReleasedThisFrame)
        {
            pushedUpButtonInFrame.Add(button);
            return true;
        }

        return false;
    }

    public static bool AnyButton()
    {
        return pushedButtonInFrame.Count > 0;
    }
    public static bool GetKeyDown(KeyCode keyCode)
    {
        if (pushDownedKeyInFrame.Contains(keyCode))
        {
            return false;
        }

        if (Input.GetKeyDown(keyCode))
        {
            pushDownedKeyInFrame.Add(keyCode);
            return true;
        }

        return false;
    }

    public static bool GetKey(KeyCode keyCode)
    {
        if (pushedKeyInFrame.Contains(keyCode))
        {
            return false;
        }

        if (Input.GetKey(keyCode))
        {
            pushedKeyInFrame.Add(keyCode);
            return true;
        }

        return false;
    }
    
    public static bool GetKeyUp(KeyCode keyCode)
    {
        if (pushUpedKeyInFrame.Contains(keyCode))
        {
            return false;
        }

        if (Input.GetKeyUp(keyCode))
        {
            pushUpedKeyInFrame.Add(keyCode);
            return true;
        }

        return false;
    }

    public static bool AnyKey()
    {
        if(pushedKeyInFrame.Count > 0)
            return true;
        return false;
    }
}