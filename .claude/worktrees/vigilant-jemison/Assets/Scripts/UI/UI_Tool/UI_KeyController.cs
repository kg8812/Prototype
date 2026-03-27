using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UI_KeyController
{
    public float initialDelay = 0.3f;
    public float repeatRate = 0.1f;

    private Dictionary<Define.UIKey, Varieties> keyVarieties = new();
    private Dictionary<Define.UIKey, Varieties> buttonVarieties = new();
    
    class Varieties
    {
        public float holdTime;
        public float repeatTimer;
        public Action onKeyPress;
    }
    
    public void Init(List<(Define.UIKey key, Action onKeyPressed)> keyPressedActions)
    {
        foreach (var keyAction in keyPressedActions)
        {
            keyVarieties.TryAdd(keyAction.key, new  Varieties
            {
                holdTime = 0f,
                repeatTimer = 0f,
                onKeyPress =  keyAction.onKeyPressed
            });
            buttonVarieties.TryAdd(keyAction.key, new Varieties
            {
                holdTime = 0f,
                repeatTimer = 0f,
                onKeyPress = keyAction.onKeyPressed
            });
        }
    }

    public void KeyUpdate()
    {
        foreach (var x in keyVarieties)
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(x.Key)))
            {
                x.Value.holdTime = 0f;
                x.Value.repeatTimer = 0f;
                x.Value.onKeyPress();
            }

            if (InputManager.GetKey(KeySettingManager.GetUIKeyCode(x.Key)))
            {
                x.Value.holdTime += Time.unscaledDeltaTime;
                if (x.Value.holdTime >= initialDelay)
                {
                    x.Value.repeatTimer += Time.unscaledDeltaTime;

                    if (x.Value.repeatTimer >= repeatRate)
                    {
                        while (x.Value.repeatTimer >= repeatRate)
                        {
                            x.Value.repeatTimer -= repeatRate;
                        }
                        x.Value.onKeyPress();
                    }
                }
            }
            else
            {
                x.Value.holdTime = 0f;
                x.Value.repeatTimer = 0f;
            }
        }
    }

    public void ButtonUpdate()
    {
        foreach (var x in buttonVarieties)
        {
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(x.Key)))
            {
                x.Value.holdTime = 0f;
                x.Value.repeatTimer = 0f;
                x.Value.onKeyPress();
            }

            if (InputManager.GetButton(KeySettingManager.GetUIButton(x.Key)))
            {
                x.Value.holdTime += Time.unscaledDeltaTime;

                if (x.Value.holdTime >= initialDelay)
                {
                    x.Value.repeatTimer += Time.unscaledDeltaTime;

                    while (x.Value.repeatTimer >= repeatRate)
                    {
                        x.Value.repeatTimer -= repeatRate;
                        x.Value.onKeyPress();
                    }
                }
            }
            else
            {
                x.Value.holdTime = 0f;
                x.Value.repeatTimer = 0f;
            }
        }
    }
}
