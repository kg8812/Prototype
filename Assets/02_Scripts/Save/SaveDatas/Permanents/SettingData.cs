using System;
using System.Collections.Generic;
using Default;
using GameStateSpace;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Save.Schema
{
    public class SettingData : ISaveData
    {
        public float[] Volumes;

        /**
         * Game setting
         */
        public LanguageType languageType;

        private Dictionary<KeyCode, Sprite> _keycodeImages;

        public Dictionary<KeyCode, Sprite> KeycodeImages => _keycodeImages;
            

        private Dictionary<ButtonControl, Sprite> _psImages;

        public Dictionary<ButtonControl, Sprite> PSImages => _psImages;

        private Dictionary<ButtonControl, Sprite> _xboxImages;

        public Dictionary<ButtonControl, Sprite> XboxImages => _xboxImages;
        
        public Dictionary<Define.GameKey, KeyCode> gameKeys;

        Dictionary<Define.UIKey, KeyCode> _UIKeys;
            
        public Dictionary<Define.UIKey, KeyCode> UIKeys => _UIKeys ??= new()
        {
            { Define.UIKey.Up, KeyCode.UpArrow },
            { Define.UIKey.Down, KeyCode.DownArrow },
            { Define.UIKey.Left, KeyCode.LeftArrow },
            { Define.UIKey.Right, KeyCode.RightArrow },
            { Define.UIKey.LeftHeader, KeyCode.Q },
            { Define.UIKey.RightHeader, KeyCode.E },
            { Define.UIKey.Select, KeyCode.Space },
            { Define.UIKey.Cancel, KeyCode.Escape },
            { Define.UIKey.Equip, KeyCode.F },
            { Define.UIKey.Skip, KeyCode.F },
        };


        private Dictionary<Define.UIKey, ButtonControl> _UIButtons;
        public Dictionary<Define.UIKey, ButtonControl> UIButtons => _UIButtons??= new()
        {
            { Define.UIKey.Up, Gamepad.current.dpad.up },
            { Define.UIKey.Down, Gamepad.current.dpad.down },
            { Define.UIKey.Left, Gamepad.current.dpad.left },
            { Define.UIKey.Right, Gamepad.current.dpad.right },
            { Define.UIKey.LeftHeader, Gamepad.current.leftShoulder },
            { Define.UIKey.RightHeader, Gamepad.current.rightShoulder },
            { Define.UIKey.Select, Gamepad.current.buttonSouth },
            { Define.UIKey.Cancel, Gamepad.current.startButton },
            { Define.UIKey.Equip, Gamepad.current.buttonWest },
            { Define.UIKey.Skip, Gamepad.current.buttonSouth },
        };


        private Dictionary<Define.GameKey, ButtonControl> _gamePadKeys;
        public Dictionary<Define.GameKey, ButtonControl> GamePadKeys => _gamePadKeys??= new()
        {
            { Define.GameKey.LeftMove, Gamepad.current.leftStick.left },
            { Define.GameKey.RightMove, Gamepad.current.leftStick.right },
            { Define.GameKey.Down, Gamepad.current.leftStick.down },
            { Define.GameKey.Up, Gamepad.current.leftStick.up },
            { Define.GameKey.Jump, Gamepad.current.buttonSouth },
        };

        private UnityEvent _onKeyChange;
        public UnityEvent OnKeyChange => _onKeyChange ??= new UnityEvent();

        public SettingData()
        {
            Volumes = new float [(int)Define.Sound.MaxCOUNT];

            for (int i = 0; i < Volumes.Length; i++)
            {
                Volumes[i] = 0.5f;
            }

            languageType = LanguageType.Korean;
            LoadKeyImages();
            gameKeys ??= new()
            {
                { Define.GameKey.LeftMove, KeyCode.LeftArrow },
                { Define.GameKey.RightMove, KeyCode.RightArrow },
                { Define.GameKey.Down, KeyCode.DownArrow },
                { Define.GameKey.Up, KeyCode.UpArrow },
                { Define.GameKey.Jump, KeyCode.Space },
            };
        }

        public void LoadKeyImages()
        {
            if (!Application.isPlaying) return;
            
            _keycodeImages ??= new Dictionary<KeyCode, Sprite>()
            {
                { KeyCode.BackQuote, ResourceUtil.Load<Sprite>("KeyCodes/quote") },
                { KeyCode.Alpha1, ResourceUtil.Load<Sprite>("KeyCodes/1") },
                { KeyCode.Alpha2, ResourceUtil.Load<Sprite>("KeyCodes/2") },
                { KeyCode.Alpha3, ResourceUtil.Load<Sprite>("KeyCodes/3") },
                { KeyCode.Alpha4, ResourceUtil.Load<Sprite>("KeyCodes/4") },
                { KeyCode.Alpha5, ResourceUtil.Load<Sprite>("KeyCodes/5") },
                { KeyCode.Alpha6, ResourceUtil.Load<Sprite>("KeyCodes/6") },
                { KeyCode.Alpha7, ResourceUtil.Load<Sprite>("KeyCodes/7") },
                { KeyCode.Alpha8, ResourceUtil.Load<Sprite>("KeyCodes/8") },
                { KeyCode.Alpha9, ResourceUtil.Load<Sprite>("KeyCodes/9") },
                { KeyCode.Alpha0, ResourceUtil.Load<Sprite>("KeyCodes/0") },
                { KeyCode.Minus, ResourceUtil.Load<Sprite>("KeyCodes/hyphen") },
                { KeyCode.Plus, ResourceUtil.Load<Sprite>("KeyCodes/equal") },
                { KeyCode.Tab, ResourceUtil.Load<Sprite>("KeyCodes/tab") },
                { KeyCode.Q, ResourceUtil.Load<Sprite>("KeyCodes/q") },
                { KeyCode.W, ResourceUtil.Load<Sprite>("KeyCodes/w") },
                { KeyCode.E, ResourceUtil.Load<Sprite>("KeyCodes/e") },
                { KeyCode.R, ResourceUtil.Load<Sprite>("KeyCodes/r") },
                { KeyCode.T, ResourceUtil.Load<Sprite>("KeyCodes/t") },
                { KeyCode.Y, ResourceUtil.Load<Sprite>("KeyCodes/y") },
                { KeyCode.U, ResourceUtil.Load<Sprite>("KeyCodes/u") },
                { KeyCode.I, ResourceUtil.Load<Sprite>("KeyCodes/i") },
                { KeyCode.O, ResourceUtil.Load<Sprite>("KeyCodes/o") },
                { KeyCode.P, ResourceUtil.Load<Sprite>("KeyCodes/p") },
                { KeyCode.LeftBracket, ResourceUtil.Load<Sprite>("KeyCodes/bracket-open") },
                { KeyCode.RightBracket, ResourceUtil.Load<Sprite>("KeyCodes/bracket-close") },
                { KeyCode.A, ResourceUtil.Load<Sprite>("KeyCodes/a") },
                { KeyCode.S, ResourceUtil.Load<Sprite>("KeyCodes/s") },
                { KeyCode.D, ResourceUtil.Load<Sprite>("KeyCodes/d") },
                { KeyCode.F, ResourceUtil.Load<Sprite>("KeyCodes/f") },
                { KeyCode.G, ResourceUtil.Load<Sprite>("KeyCodes/g") },
                { KeyCode.H, ResourceUtil.Load<Sprite>("KeyCodes/h") },
                { KeyCode.J, ResourceUtil.Load<Sprite>("KeyCodes/j") },
                { KeyCode.K, ResourceUtil.Load<Sprite>("KeyCodes/k") },
                { KeyCode.L, ResourceUtil.Load<Sprite>("KeyCodes/l") },
                { KeyCode.Semicolon, ResourceUtil.Load<Sprite>("KeyCodes/semi-colon") },
                { KeyCode.LeftShift, ResourceUtil.Load<Sprite>("KeyCodes/shift") },
                { KeyCode.Z, ResourceUtil.Load<Sprite>("KeyCodes/z") },
                { KeyCode.X, ResourceUtil.Load<Sprite>("KeyCodes/x") },
                { KeyCode.C, ResourceUtil.Load<Sprite>("KeyCodes/c") },
                { KeyCode.V, ResourceUtil.Load<Sprite>("KeyCodes/v") },
                { KeyCode.B, ResourceUtil.Load<Sprite>("KeyCodes/b") },
                { KeyCode.N, ResourceUtil.Load<Sprite>("KeyCodes/n") },
                { KeyCode.M, ResourceUtil.Load<Sprite>("KeyCodes/m") },
                { KeyCode.Comma, ResourceUtil.Load<Sprite>("KeyCodes/comma") },
                { KeyCode.LeftControl, ResourceUtil.Load<Sprite>("KeyCodes/ctrl") },
                { KeyCode.LeftAlt, ResourceUtil.Load<Sprite>("KeyCodes/alt") },
                { KeyCode.Space, ResourceUtil.Load<Sprite>("KeyCodes/space") },
                { KeyCode.LeftArrow, ResourceUtil.Load<Sprite>("KeyCodes/arrow-left") },
                { KeyCode.UpArrow, ResourceUtil.Load<Sprite>("KeyCodes/arrow-up") },
                { KeyCode.DownArrow, ResourceUtil.Load<Sprite>("KeyCodes/arrow-down") },
                { KeyCode.RightArrow, ResourceUtil.Load<Sprite>("KeyCodes/arrow-right") },
                { KeyCode.Mouse0, ResourceUtil.Load<Sprite>("KeyCodes/mouse-left") },
                { KeyCode.Mouse1, ResourceUtil.Load<Sprite>("KeyCodes/mouse-right") },
                { KeyCode.Mouse2, ResourceUtil.Load<Sprite>("KeyCodes/mouse-middle") },
                { KeyCode.Mouse3, ResourceUtil.Load<Sprite>("KeyCodes/mouse-g1") },
                { KeyCode.Mouse4, ResourceUtil.Load<Sprite>("KeyCodes/mouse-g2") },
            };
        }

        public void LoadGamePadImages()
        {
            if (!Application.isPlaying) return;

            _psImages ??= new()
            {
                {Gamepad.current.dpad.left, ResourceUtil.Load<Sprite>("KeyCodes/ps-arrow-left")},
                {Gamepad.current.dpad.right, ResourceUtil.Load<Sprite>("KeyCodes/ps-arrow-right")},
                {Gamepad.current.dpad.up, ResourceUtil.Load<Sprite>("KeyCodes/ps-arrow-up")},
                {Gamepad.current.dpad.down, ResourceUtil.Load<Sprite>("KeyCodes/ps-arrow-down")},
                {Gamepad.current.leftShoulder,ResourceUtil.Load<Sprite>("KeyCodes/ps-l1")},
                {Gamepad.current.leftTrigger,ResourceUtil.Load<Sprite>("KeyCodes/ps-l2")},
                {Gamepad.current.leftStickButton,ResourceUtil.Load<Sprite>("KeyCodes/ps-l3-click")},
                {Gamepad.current.leftStick.up,ResourceUtil.Load<Sprite>("KeyCodes/ps-l3-up")},
                {Gamepad.current.leftStick.left,ResourceUtil.Load<Sprite>("KeyCodes/ps-l3-left")},
                {Gamepad.current.leftStick.down,ResourceUtil.Load<Sprite>("KeyCodes/ps-l3-down")},
                {Gamepad.current.leftStick.right,ResourceUtil.Load<Sprite>("KeyCodes/ps-l3-right")},
                {Gamepad.current.rightShoulder,ResourceUtil.Load<Sprite>("KeyCodes/ps-r1")},
                {Gamepad.current.rightTrigger,ResourceUtil.Load<Sprite>("KeyCodes/ps-r2")},
                {Gamepad.current.rightStickButton,ResourceUtil.Load<Sprite>("KeyCodes/ps-r3-click")},
                {Gamepad.current.rightStick.up,ResourceUtil.Load<Sprite>("KeyCodes/ps-r3-up")},
                {Gamepad.current.rightStick.left,ResourceUtil.Load<Sprite>("KeyCodes/ps-r3-left")},
                {Gamepad.current.rightStick.down,ResourceUtil.Load<Sprite>("KeyCodes/ps-r3-down")},
                {Gamepad.current.rightStick.right,ResourceUtil.Load<Sprite>("KeyCodes/ps-r3-right")},
                {Gamepad.current.buttonEast,ResourceUtil.Load<Sprite>("KeyCodes/ps-o")},
                {Gamepad.current.buttonWest,ResourceUtil.Load<Sprite>("KeyCodes/ps-square")},
                {Gamepad.current.buttonSouth,ResourceUtil.Load<Sprite>("KeyCodes/ps-x")},
                {Gamepad.current.buttonNorth,ResourceUtil.Load<Sprite>("KeyCodes/ps-triangle")},
                {Gamepad.current.startButton,ResourceUtil.Load<Sprite>("KeyCodes/ps-options")},
                {Gamepad.current.selectButton,ResourceUtil.Load<Sprite>("KeyCodes/ps-share")},
            };

            _xboxImages ??= new()
            {
                { Gamepad.current.dpad.left, ResourceUtil.Load<Sprite>("KeyCodes/xbox-arrow-left") },
                { Gamepad.current.dpad.right, ResourceUtil.Load<Sprite>("KeyCodes/xbox-arrow-right") },
                { Gamepad.current.dpad.up, ResourceUtil.Load<Sprite>("KeyCodes/xbox-arrow-up") },
                { Gamepad.current.dpad.down, ResourceUtil.Load<Sprite>("KeyCodes/xbox-arrow-down") },
                { Gamepad.current.leftShoulder, ResourceUtil.Load<Sprite>("KeyCodes/xbox-lb") },
                { Gamepad.current.leftTrigger, ResourceUtil.Load<Sprite>("KeyCodes/xbox-lt") },
                { Gamepad.current.leftStickButton, ResourceUtil.Load<Sprite>("KeyCodes/xbox-ls-click") },
                { Gamepad.current.leftStick.up, ResourceUtil.Load<Sprite>("KeyCodes/xbox-ls-up") },
                { Gamepad.current.leftStick.left, ResourceUtil.Load<Sprite>("KeyCodes/xbox-ls-left") },
                { Gamepad.current.leftStick.down, ResourceUtil.Load<Sprite>("KeyCodes/xbox-ls-down") },
                { Gamepad.current.leftStick.right, ResourceUtil.Load<Sprite>("KeyCodes/xbox-ls-right") },
                { Gamepad.current.rightShoulder, ResourceUtil.Load<Sprite>("KeyCodes/xbox-rb") },
                { Gamepad.current.rightTrigger, ResourceUtil.Load<Sprite>("KeyCodes/xbox-rt") },
                { Gamepad.current.rightStickButton, ResourceUtil.Load<Sprite>("KeyCodes/xbox-rs-click") },
                { Gamepad.current.rightStick.up, ResourceUtil.Load<Sprite>("KeyCodes/xbox-rs-up") },
                { Gamepad.current.rightStick.left, ResourceUtil.Load<Sprite>("KeyCodes/xbox-rs-left") },
                { Gamepad.current.rightStick.down, ResourceUtil.Load<Sprite>("KeyCodes/xbox-rs-down") },
                { Gamepad.current.rightStick.right, ResourceUtil.Load<Sprite>("KeyCodes/xbox-rs-right") },
                { Gamepad.current.buttonEast, ResourceUtil.Load<Sprite>("KeyCodes/xbox-b") },
                { Gamepad.current.buttonWest, ResourceUtil.Load<Sprite>("KeyCodes/xbox-x") },
                { Gamepad.current.buttonSouth, ResourceUtil.Load<Sprite>("KeyCodes/xbox-a") },
                { Gamepad.current.buttonNorth, ResourceUtil.Load<Sprite>("KeyCodes/xbox-y") },
                { Gamepad.current.startButton, ResourceUtil.Load<Sprite>("KeyCodes/xbox-menu") },
                { Gamepad.current.selectButton, ResourceUtil.Load<Sprite>("KeyCodes/xbox-view") },
            };
        }
        
        public void OnLoaded()
        {
            Debug.Log(Volumes.Length);

            foreach (Define.Sound sound in Enum.GetValues(typeof(Define.Sound)))
            {
                Debug.Log("Volume : " +(int)sound);
                
                if (sound != Define.Sound.MaxCOUNT)
                {
                    GameManager.Sound.ChangeVolume(Volumes[(int)sound],sound);
                }
            }
        }

        public void Initialize()
        {
            Volumes ??= new float [(int)Define.Sound.MaxCOUNT];
            
            for (int i = 0; i < Volumes.Length; i++)
            {
                Volumes[i] = 0.5f;
            }
            
            foreach (Define.Sound sound in Enum.GetValues(typeof(Define.Sound)))
            {
                if (sound != Define.Sound.MaxCOUNT)
                {
                    GameManager.Sound.ChangeVolume(Volumes[(int)sound],sound);
                }
            }
            
            LoadKeyImages();
        }

        public void BeforeSave()
        {
            for (int i = 0; i < Volumes.Length; i++)
            {
                Volumes[i] = GameManager.Sound.volume[i];
            }
        }

        public Sprite GetGameKeyImage(Define.GameKey key)
        {
            Sprite image;
            switch (GameManager.instance.currentInputType)
            {
                case InputType.KeyBoard:
                    LoadKeyImages();
                    if (gameKeys.ContainsKey(key) && KeycodeImages.TryGetValue(gameKeys[key], out image))
                    {
                        return image;
                    }

                    break;
                case InputType.GamePad:
                    var gamepad = Gamepad.current;

                    if (gamepad != null)
                    {
                        LoadGamePadImages();
                        string name = (gamepad.name + gamepad.displayName).ToLower(); // 두 값을 하나로 합쳐서 체크

                        if (name.Contains("dualshock") || name.Contains("dualsense") || name.Contains("playstation"))
                        {
                            if (GamePadKeys.ContainsKey(key) && PSImages.TryGetValue(GamePadKeys[key], out image))
                            {
                                return image;
                            }
                        }
                        else if (name.Contains("xbox"))
                        {
                            if (GamePadKeys.ContainsKey(key) && XboxImages.TryGetValue(GamePadKeys[key], out image))
                            {
                                return image;
                            }
                        }
                    }
                    break;
            }
            

            return null;
        }

        public Sprite GetUIKeyImage(Define.UIKey key)
        {
            Sprite image;
            switch (GameManager.instance.currentInputType)
            {
                case InputType.KeyBoard:
                    LoadKeyImages();
                    if (UIKeys.ContainsKey(key) && KeycodeImages.TryGetValue(UIKeys[key], out image))
                    {
                        return image;
                    }

                    break;
                case InputType.GamePad:
                    var gamepad = Gamepad.current;

                    if (gamepad != null)
                    {
                        LoadGamePadImages();
                        string name = (gamepad.name + gamepad.displayName).ToLower(); // 두 값을 하나로 합쳐서 체크

                        if (name.Contains("dualshock") || name.Contains("dualsense") || name.Contains("playstation"))
                        {
                            if (UIButtons.ContainsKey(key) && PSImages.TryGetValue(UIButtons[key], out image))
                            {
                                return image;
                            }
                        }
                        else if (name.Contains("xbox"))
                        {
                            if (UIButtons.ContainsKey(key) && XboxImages.TryGetValue(UIButtons[key], out image))
                            {
                                return image;
                            }
                        }
                    }
                    break;
            }

            return null;
        }
        public void SetGameKey(Define.GameKey gameKey, KeyCode keyCode)
        {
            KeyCode lastKey = gameKeys[gameKey];
            bool isFound = false;
            Define.GameKey tempKey = gameKey;
            foreach (var x in gameKeys.Keys)
            {
                if (gameKeys[x] == keyCode)
                {
                    isFound = true;
                    tempKey = x;
                    break;
                }
            }

            if (isFound)
            {
                gameKeys[tempKey] = lastKey;
            }

            gameKeys[gameKey] = keyCode;

            OnKeyChange?.Invoke();
        }

        public void ApplyPreset(int number)
        {
            var keys = GetKeySettingPreset(number);

            keys.ForEach(x => { gameKeys[x.Item1] = x.Item2; });
            OnKeyChange?.Invoke();
        }

        List<(Define.GameKey, KeyCode)> GetKeySettingPreset(int number)
        {
            if (number == 2)
            {
                return new()
                {
                    (Define.GameKey.LeftMove, KeyCode.A),
                    (Define.GameKey.RightMove, KeyCode.D),
                    (Define.GameKey.Down, KeyCode.S),
                    (Define.GameKey.Up, KeyCode.W),
                    (Define.GameKey.Jump, KeyCode.Space),
                };
            }

            return new()
            {
                (Define.GameKey.LeftMove, KeyCode.LeftArrow),
                (Define.GameKey.RightMove, KeyCode.RightArrow),
                (Define.GameKey.Down, KeyCode.DownArrow),
                (Define.GameKey.Up, KeyCode.UpArrow),
                (Define.GameKey.Jump, KeyCode.Space),
            };
        }
    }
}