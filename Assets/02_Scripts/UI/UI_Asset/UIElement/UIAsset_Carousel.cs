using System.Collections.Generic;
using Apis.UI.Focus;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Apis.UI
{
    public class UIAsset_Carousel : UIEffector
    {
        public ConstText value;

        [ShowInInspector] public List<int> CarList = new();

        [SerializeField] private NavigationMode mode;
        [SerializeField] private int defaultInd;
        private int curInd;

        public UnityEvent<int> ValueChanged = new();


        public void MoveTo(int ind)
        {
            if (curInd != ind && 0 <= ind && ind < CarList.Count)
            {
                curInd = ind;
                UpdateValue();
            }
        }

        public void MovePre()
        {
            if (curInd <= 0)
                curInd = CarList.Count - 1;
            else
                curInd--;
            UpdateValue();
        }

        public void MoveNext()
        {
            if (curInd >= CarList.Count - 1)
                curInd = 0;
            else
                curInd++;
            UpdateValue();
        }

        private void UpdateValue()
        {
            value.ChangeId(CarList[curInd]);
            ValueChanged.Invoke(curInd);
        }


        public override void KeyControl()
        {
            if (mode == NavigationMode.Horizontal)
            {
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left))) MovePre();
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right))) MoveNext();
            }
            else
            {
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Up))) MovePre();
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Down))) MoveNext();
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();

            if (mode == NavigationMode.Horizontal)
            {
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Left))) MovePre();
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Right))) MoveNext();
            }
            else
            {
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Up))) MovePre();
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Down))) MoveNext();
            }
        }
    }
}