using System;
using System.Collections.Generic;
using Apis.UI.Focus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UIAsset_Slider: UIEffector
    {
        public Slider slider;
        public NavigationMode mode;
        [Range(0,1)] public float movePower = 0.01f;

        private float curValue;
        [SerializeField] private UI_KeyController _keyController;
        
        public override void Init()
        {
            base.Init();
            curValue = slider.value;
            if (_keyController == null)
            {
                _keyController = new();
            }

            if (mode == NavigationMode.Horizontal)
            {
                _keyController.Init(new List<(Define.UIKey key, Action onKeyPressed)>
                {
                    (Define.UIKey.Left,MovePre),
                    (Define.UIKey.Right,MoveNext),
                });
            }
            else
            {
                _keyController.Init(new List<(Define.UIKey key, Action onKeyPressed)>
                {
                    (Define.UIKey.Down, MovePre),
                    (Define.UIKey.Up, MoveNext),
                });
            }
        }

        public void MovePre()
        {
            curValue = Mathf.Clamp01(curValue - movePower);
            UpdateValue();
        }

        public void MoveNext()
        {
            curValue = Mathf.Clamp01(curValue + movePower);
            UpdateValue();
        }

        private void UpdateValue()
        {
            slider.value = curValue;
        }
        
        
        public override void KeyControl()
        {
            base.KeyControl();
            _keyController.KeyUpdate();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            
            _keyController.ButtonUpdate();
        }
    }
}