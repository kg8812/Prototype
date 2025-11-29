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
        private Dictionary<Define.GameKey, ButtonControl> _gamePadKeys;

        private UnityEvent _onKeyChange;


        private Dictionary<Define.UIKey, ButtonControl> _UIButtons;

        private Dictionary<Define.UIKey, KeyCode> _UIKeys;

        public Dictionary<Define.GameKey, KeyCode> gameKeys;

        /**
         * Game setting
         */
        public LanguageType languageType;

        public float[] Volumes;

        public SettingData()
        {
            Volumes = new float [(int)Define.Sound.MaxCOUNT];

            for (var i = 0; i < Volumes.Length; i++) Volumes[i] = 0.5f;

            languageType = LanguageType.Korean;
            LoadKeyImages();
            gameKeys ??= new Dictionary<Define.GameKey, KeyCode>
            {
                { Define.GameKey.LeftMove, KeyCode.LeftArrow },
                { Define.GameKey.RightMove, KeyCode.RightArrow },
                { Define.GameKey.Down, KeyCode.DownArrow },
                { Define.GameKey.Up, KeyCode.UpArrow },
                { Define.GameKey.Jump, KeyCode.Space },
                { Define.GameKey.ActiveSkill, KeyCode.D },
                { Define.GameKey.Attack, KeyCode.Z },
                {Define.GameKey.Dash,KeyCode.LeftShift},
            };
        }

        public Dictionary<KeyCode, Sprite> KeycodeImages { get; private set; }

        public Dictionary<ButtonControl, Sprite> PSImages { get; private set; }

        public Dictionary<ButtonControl, Sprite> XboxImages { get; private set; }

        public Dictionary<Define.UIKey, KeyCode> UIKeys => _UIKeys ??= new Dictionary<Define.UIKey, KeyCode>
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
            { Define.UIKey.Skip, KeyCode.F }
        };

        public Dictionary<Define.UIKey, ButtonControl> UIButtons => _UIButtons ??=
            new Dictionary<Define.UIKey, ButtonControl>
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
                { Define.UIKey.Skip, Gamepad.current.buttonSouth }
            };

        public Dictionary<Define.GameKey, ButtonControl> GamePadKeys => _gamePadKeys ??=
            new Dictionary<Define.GameKey, ButtonControl>
            {
                { Define.GameKey.LeftMove, Gamepad.current.leftStick.left },
                { Define.GameKey.RightMove, Gamepad.current.leftStick.right },
                { Define.GameKey.Down, Gamepad.current.leftStick.down },
                { Define.GameKey.Up, Gamepad.current.leftStick.up },
                { Define.GameKey.Jump, Gamepad.current.buttonSouth },
                { Define.GameKey.ActiveSkill, Gamepad.current.leftTrigger },
                { Define.GameKey.Attack, Gamepad.current.buttonWest },
                {Define.GameKey.Dash,Gamepad.current.buttonEast},
            };

        public UnityEvent OnKeyChange => _onKeyChange ??= new UnityEvent();

        public void OnLoaded()
        {
            Debug.Log(Volumes.Length);

            foreach (Define.Sound sound in Enum.GetValues(typeof(Define.Sound)))
            {
                Debug.Log("Volume : " + (int)sound);

                if (sound != Define.Sound.MaxCOUNT) GameManager.Sound.ChangeVolume(Volumes[(int)sound], sound);
            }
        }

        public void Initialize()
        {
            Volumes ??= new float [(int)Define.Sound.MaxCOUNT];

            for (var i = 0; i < Volumes.Length; i++) Volumes[i] = 0.5f;

            foreach (Define.Sound sound in Enum.GetValues(typeof(Define.Sound)))
                if (sound != Define.Sound.MaxCOUNT)
                    GameManager.Sound.ChangeVolume(Volumes[(int)sound], sound);

            LoadKeyImages();
        }

        public void BeforeSave()
        {
            for (var i = 0; i < Volumes.Length; i++) Volumes[i] = GameManager.Sound.volume[i];
        }

        public void LoadKeyImages()
        {
            if (!Application.isPlaying) return;

            // 키보드 키 이미지 로드
            KeycodeImages ??= new Dictionary<KeyCode, Sprite>
            {
                //{ KeyCode.BackQuote, ResourceUtil.Load<Sprite>("KeyCodes/quote") },
            };
        }

        public void LoadGamePadImages()
        {
            if (!Application.isPlaying) return;

            // 플스패드 키 이미지 로드
            PSImages ??= new Dictionary<ButtonControl, Sprite>
            {
                //{ Gamepad.current.dpad.left, ResourceUtil.Load<Sprite>("KeyCodes/ps-arrow-left") },
            };

            // 엑박패드 키 이미지 로드
            XboxImages ??= new Dictionary<ButtonControl, Sprite>
            {
                //{ Gamepad.current.dpad.left, ResourceUtil.Load<Sprite>("KeyCodes/xbox-arrow-left") },
            };
        }

        public Sprite GetGameKeyImage(Define.GameKey key)
        {
            Sprite image;
            switch (GameManager.instance.currentInputType)
            {
                case InputType.KeyBoard:
                    LoadKeyImages();
                    if (gameKeys.ContainsKey(key) && KeycodeImages.TryGetValue(gameKeys[key], out image)) return image;

                    break;
                case InputType.GamePad:
                    var gamepad = Gamepad.current;

                    if (gamepad != null)
                    {
                        LoadGamePadImages();
                        var name = (gamepad.name + gamepad.displayName).ToLower(); // 두 값을 하나로 합쳐서 체크

                        if (name.Contains("dualshock") || name.Contains("dualsense") || name.Contains("playstation"))
                        {
                            if (GamePadKeys.ContainsKey(key) && PSImages.TryGetValue(GamePadKeys[key], out image))
                                return image;
                        }
                        else if (name.Contains("xbox"))
                        {
                            if (GamePadKeys.ContainsKey(key) && XboxImages.TryGetValue(GamePadKeys[key], out image))
                                return image;
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
                    if (UIKeys.ContainsKey(key) && KeycodeImages.TryGetValue(UIKeys[key], out image)) return image;

                    break;
                case InputType.GamePad:
                    var gamepad = Gamepad.current;

                    if (gamepad != null)
                    {
                        LoadGamePadImages();
                        var name = (gamepad.name + gamepad.displayName).ToLower(); // 두 값을 하나로 합쳐서 체크

                        if (name.Contains("dualshock") || name.Contains("dualsense") || name.Contains("playstation"))
                        {
                            if (UIButtons.ContainsKey(key) && PSImages.TryGetValue(UIButtons[key], out image))
                                return image;
                        }
                        else if (name.Contains("xbox"))
                        {
                            if (UIButtons.ContainsKey(key) && XboxImages.TryGetValue(UIButtons[key], out image))
                                return image;
                        }
                    }

                    break;
            }

            return null;
        }

        public void SetGameKey(Define.GameKey gameKey, KeyCode keyCode)
        {
            var lastKey = gameKeys[gameKey];
            var isFound = false;
            var tempKey = gameKey;
            foreach (var x in gameKeys.Keys)
                if (gameKeys[x] == keyCode)
                {
                    isFound = true;
                    tempKey = x;
                    break;
                }

            if (isFound) gameKeys[tempKey] = lastKey;

            gameKeys[gameKey] = keyCode;

            OnKeyChange?.Invoke();
        }

        public void ApplyPreset(int number)
        {
            var keys = GetKeySettingPreset(number);

            keys.ForEach(x => { gameKeys[x.Item1] = x.Item2; });
            OnKeyChange?.Invoke();
        }

        private List<(Define.GameKey, KeyCode)> GetKeySettingPreset(int number)
        {
            if (number == 2)
                return new List<(Define.GameKey, KeyCode)>
                {
                    (Define.GameKey.LeftMove, KeyCode.A),
                    (Define.GameKey.RightMove, KeyCode.D),
                    (Define.GameKey.Down, KeyCode.S),
                    (Define.GameKey.Up, KeyCode.W),
                    (Define.GameKey.Jump, KeyCode.Space),
                    (Define.GameKey.ActiveSkill, KeyCode.R),
                    (Define.GameKey.Attack, KeyCode.Mouse0),
                    (Define.GameKey.Dash, KeyCode.LeftShift),
                };

            return new List<(Define.GameKey, KeyCode)>
            {
                (Define.GameKey.LeftMove, KeyCode.LeftArrow),
                (Define.GameKey.RightMove, KeyCode.RightArrow),
                (Define.GameKey.Down, KeyCode.DownArrow),
                (Define.GameKey.Up, KeyCode.UpArrow),
                (Define.GameKey.Jump, KeyCode.Space),
                (Define.GameKey.ActiveSkill, KeyCode.D),
                (Define.GameKey.Attack, KeyCode.Z),
                (Define.GameKey.Dash, KeyCode.LeftShift),
            };
        }
    }
}