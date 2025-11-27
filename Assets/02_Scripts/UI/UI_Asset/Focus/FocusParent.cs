using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Apis.UI.Focus
{
    public delegate void MoveEvent(int value);

    public enum NavigationMode
    {
        Horizontal,
        Vertical,
        Inventory
    }

    [Serializable]
    public struct TableNavigationData
    {
        public int x, y;
        public bool isLeftLoop, isRightLoop, isUpLoop, isDownLoop;
        public MoveEvent moveLeft;
        public MoveEvent moveRight;
        public MoveEvent moveUp;
        public MoveEvent moveDown;

        public bool fixedY;
        // public UnityEvent<int> moveLeft;
        // public UnityEvent<int> moveRight;
    }

    public class FocusParent : MonoBehaviour, IController
    {
        public bool isFocusSelect;

        [Tooltip("true로 설정하면 클릭(Select)해야만 포커스를 받는 방식으로, false이면 마우스 호버만으로도 포커스를 받는 방식으로 동작을 변경할 수 있습니다.")]
        public bool canNoneFocus;

        [Tooltip(
            "그룹 내에 아무것도 선택되지 않은 상태를 허용할지.\n 체크 시: UI 바깥을 클릭하면 모든 요소의 포커스가 해제될 수 있습니다.\n 체크 해제 시:이 그룹 내 요소 중 하나는 반드시 포커스를 받아야 합니다.")]
        public bool canLoop;

        public bool useNumberKey;
        public NavigationMode navigation;

        [ShowIf("navigation", NavigationMode.Inventory)]
        public TableNavigationData tableData;

        public bool isQE;
        [SerializeField] public List<UIElement> focusList = new();

        public UnityEvent<int> FocusChanged;

        private bool _isFocused;
        // public UnityEvent<int> FocusSelected;

        private List<int> _labels = new();
        private int _lastLabel;

        private readonly Dictionary<UIElement, Action<UIElementState>> eventHandlers = new();

        [HideInInspector] public IFocusGroup FocusGroup;

        private bool inited;
        public int curId { get; private set; }

        public void Reset(bool isHard = false)
        {
            _lastLabel = 0;
            _isFocused = false;
            if (focusList == null)
                focusList = new List<UIElement>();
            else if (isHard)
                focusList.Clear();

            if (_labels == null)
                _labels = new List<int>();
            else
                _labels.Clear();
        }

        private void Start()
        {
            InitCheck();
        }


        public void KeyControl()
        {
            // Debug.LogError("www");
            if (_isFocused)
                // Debug.LogError($"cur id : {curId}");
                if (0 <= curId && curId < focusList.Count)
                    focusList[curId].KeyControl();
            if (isQE)
            {
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.LeftHeader)))
                    MoveFocus(true);
                else if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.RightHeader)))
                    MoveFocus();
                else if (useNumberKey)
                    for (var i = 0; i < focusList.Count; i++)
                        // keycode alpha 는 50부터 시작
                        if (InputManager.GetKeyDown((KeyCode)(i + 50)))
                            MoveTo(i);
            }
            else
            {
                if (navigation == NavigationMode.Horizontal)
                {
                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left))) MoveFocus(true);
                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right))) MoveFocus();
                }
                else if (navigation == NavigationMode.Vertical)
                {
                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Up))) MoveFocus(true);
                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Down))) MoveFocus();
                }
                else if (navigation == NavigationMode.Inventory)
                {
                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Up)))
                    {
                        // 가장 상단에 위치
                        if (curId < tableData.x)
                        {
                            if (tableData.isUpLoop)
                                MoveTo(curId + (focusList.Count - curId + 1) / tableData.x * tableData.x);
                            else if (tableData.moveUp != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveUp.Invoke(curId % tableData.x);
                                }
                        }
                        else
                        {
                            MoveTo(curId - tableData.x);
                        }
                    }

                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Down)))
                    {
                        // 가장 하단에 위치
                        if (curId >= focusList.Count - tableData.x)
                        {
                            if (tableData.isDownLoop)
                                MoveTo(curId % tableData.x);
                            else if (tableData.moveDown != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveDown.Invoke(curId % tableData.x);
                                }
                        }
                        else
                        {
                            MoveTo(curId + tableData.x);
                        }
                    }

                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Left)))
                    {
                        // if (!inventory.MoveLeft(checkLast: true))
                        // {
                        //     if (canNoneFocus)
                        //     {
                        //         // Debug.Log("MoveRight");
                        //         FocusReset();
                        //         tableData.moveLeft.Invoke(curId / tableData.x);
                        //     }
                        // }
                        // 테이블에 가장 왼쪽 열에 분포함.
                        if (curId % tableData.x == 0)
                        {
                            if (tableData.isLeftLoop)
                                MoveTo(Mathf.Max(curId + tableData.x - 1, focusList.Count - 1));
                            else if (tableData.moveLeft != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveLeft.Invoke(curId / tableData.x);
                                }
                            // 만약에 can None Focus가 false라면 focus된 개체가 무조건 존재해야 한다는 의미이니 이벤트 발생 x.
                        }
                        else
                        {
                            // Debug.Log("왼쪽으로");
                            MoveTo(curId - 1);
                        }
                    }

                    if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Right)))
                    {
                        // 테이블에 가장 오른쪽 열에 분포함 (가장 하단은 마지막 요소도 포함)
                        if (curId == focusList.Count - 1 || (curId + 1) % tableData.x == 0)
                        {
                            if (tableData.isRightLoop)
                                MoveTo(curId / tableData.x * tableData.x);
                            else if (tableData.moveRight != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveRight.Invoke(curId / tableData.x);
                                }
                        }
                        else
                        {
                            MoveTo(curId + 1);
                        }
                    }
                }
            }
        }

        public void GamePadControl()
        {
            if (_isFocused)
                // Debug.LogError($"cur id : {curId}");
                if (0 <= curId && curId < focusList.Count)
                    focusList[curId].GamePadControl();
            if (isQE)
            {
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.LeftHeader)))
                    MoveFocus(true);
                else if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.RightHeader)))
                    MoveFocus();
            }
            else
            {
                if (navigation == NavigationMode.Horizontal)
                {
                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Left))) MoveFocus(true);
                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Right))) MoveFocus();
                }
                else if (navigation == NavigationMode.Vertical)
                {
                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Up))) MoveFocus(true);
                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Down))) MoveFocus();
                }
                else if (navigation == NavigationMode.Inventory)
                {
                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Up)))
                    {
                        // 가장 상단에 위치
                        if (curId < tableData.x)
                        {
                            if (tableData.isUpLoop)
                                MoveTo(curId + (focusList.Count - curId + 1) / tableData.x * tableData.x);
                            else if (tableData.moveUp != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveUp.Invoke(curId % tableData.x);
                                }
                        }
                        else
                        {
                            MoveTo(curId - tableData.x);
                        }
                    }

                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Down)))
                    {
                        // 가장 하단에 위치
                        if (curId >= focusList.Count - tableData.x)
                        {
                            if (tableData.isDownLoop)
                                MoveTo(curId % tableData.x);
                            else if (tableData.moveDown != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveDown.Invoke(curId % tableData.x);
                                }
                        }
                        else
                        {
                            MoveTo(curId + tableData.x);
                        }
                    }

                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Left)))
                    {
                        if (curId % tableData.x == 0)
                        {
                            if (tableData.isLeftLoop)
                                MoveTo(Mathf.Max(curId + tableData.x - 1, focusList.Count - 1));
                            else if (tableData.moveLeft != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveLeft.Invoke(curId / tableData.x);
                                }
                            // 만약에 can None Focus가 false라면 focus된 개체가 무조건 존재해야 한다는 의미이니 이벤트 발생 x.
                        }
                        else
                        {
                            // Debug.Log("왼쪽으로");
                            MoveTo(curId - 1);
                        }
                    }

                    if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Right)))
                    {
                        // 테이블에 가장 오른쪽 열에 분포함 (가장 하단은 마지막 요소도 포함)
                        if (curId == focusList.Count - 1 || (curId + 1) % tableData.x == 0)
                        {
                            if (tableData.isRightLoop)
                                MoveTo(curId / tableData.x * tableData.x);
                            else if (tableData.moveRight != null)
                                if (canNoneFocus)
                                {
                                    FocusReset();
                                    tableData.moveRight.Invoke(curId / tableData.x);
                                }
                        }
                        else
                        {
                            MoveTo(curId + 1);
                        }
                    }
                }
            }
        }

        public void InitCheck()
        {
            if (inited) return;
            Init();
        }


        protected virtual void Init()
        {
            if (inited) return;
            inited = true;

            FocusChanged = new UnityEvent<int>();
            // FocusSelected = new();
            Reset();
            foreach (var focusItem in focusList)
            {
                focusItem.InitCheck();
                var id = RegisterElement(focusItem, isAlreadyList: true);
                if (UIElement.EqualSt(focusItem.ElState, isFocusSelect ? UIElementState.Select : UIElementState.Hover))
                {
                    MoveToLabel(id);
                    _isFocused = true;
                }
            }

            ChangeFocusType(isFocusSelect);
        }

        public void FocusReset()
        {
            // if (!_isFocused) return;
            _isFocused = false;
            foreach (var focus in focusList)
            {
                focus.SelectOff(true);
                focus.HoverOff(true);
            }

            if (!canNoneFocus) MoveTo(0);
        }

        public int RegisterElement(UIElement el, int index = -1, bool isAlreadyList = false)
        {
            el.focusOffByParent = !canNoneFocus;
            var label = CreateLabel();
            if (index == -1)
            {
                if (!isAlreadyList)
                    focusList.Add(el);
                _labels.Add(label);
            }
            else
            {
                if (!isAlreadyList)
                    focusList.Insert(index, el);
                _labels.Insert(index, label);
                // if (index <= _curId)
                //     _curId++;
            }

            RegisterMouseEvent(el, label);
            if (navigation == NavigationMode.Inventory)
                if (focusList.Count > tableData.x * tableData.y)
                {
                    if (tableData.fixedY)
                        tableData.x += 1;
                    else
                        tableData.y += 1;
                }

            return label;
        }

        public bool RemoveElement(UIElement el)
        {
            for (var i = 0; i < focusList.Count; i++)
                if (ReferenceEquals(focusList[i], el))
                {
                    RemoveMouseEvent(el);
                    focusList.RemoveAt(i);
                    _labels.RemoveAt(i);

                    if (navigation == NavigationMode.Inventory)
                    {
                        if (tableData.fixedY)
                        {
                            if (focusList.Count <= (tableData.x - 1) * tableData.y) tableData.x -= 1;
                        }
                        else
                        {
                            if (focusList.Count <= tableData.x * (tableData.y - 1)) tableData.y -= 1;
                        }
                    }

                    return true;
                }

            return false;
        }

        public void ChangeFocusType(bool isSelect)
        {
            if (isFocusSelect == isSelect) return;
            isFocusSelect = isSelect;
            foreach (var el in focusList)
            {
                el.isFocusSelect = isSelect;
                el.UpdateFocusType();
            }
        }

        public void ChangeCanNoneFocus(bool canNone)
        {
            if (canNoneFocus == canNone) return;
            canNoneFocus = canNone;
            foreach (var focusItem in focusList) focusItem.focusOffByParent = !canNoneFocus;
        }

        public void AllDisableToggle(bool isDisable)
        {
            foreach (var el in focusList)
                if (isDisable)
                    el.DisableOn();
                else
                    el.DisableOff();
        }

        public void AllFrozenToggle(bool isFrozen)
        {
            foreach (var el in focusList) el.isFrozen = isFrozen;
        }

        private int CreateLabel()
        {
            return _lastLabel++;
        }

        private void RegisterMouseEvent(UIElement el, int label)
        {
            Action<UIElementState> callback = st => UIElementStateChanged(st, label);
            eventHandlers[el] = callback;
            el.StateChanged += callback;
        }

        private void RemoveMouseEvent(UIElement el)
        {
            if (eventHandlers.TryGetValue(el, out var callback))
            {
                el.StateChanged -= callback;
                eventHandlers.Remove(el);
            }
        }

        private void UIElementStateChanged(UIElementState elState, int label)
        {
            // Debug.LogError($"state changed to {elState} {label}");
            if (isFocusSelect)
            {
                if (UIElement.EqualSt(elState, UIElementState.Select)) MoveToLabel(label);
            }
            else
            {
                if (UIElement.EqualSt(elState, UIElementState.Hover)) MoveToLabel(label);
                // if (UIElement.EqualSt(elState, UIElementState.Select))
                // {
                //     SelectFocused(label);
                // }
            }
        }

        // focus = hover 일때만 사용
        // public void SelectFocused(int index)
        // {
        //     FocusSelected.Invoke(FindIndexByLabel(index));
        // }

        public void MoveFocus(bool isBack = false)
        {
            if (!_isFocused)
            {
                if (!canNoneFocus)
                {
                    Debug.LogError("불가능.");
                    return;
                }

                MoveTo(curId);
            }
            else
            {
                if (isBack)
                {
                    if (curId - 1 < 0)
                    {
                        if (canLoop)
                            MoveTo(focusList.Count - 1);
                    }
                    else
                    {
                        MoveTo(curId - 1);
                    }
                }
                else
                {
                    if (curId + 1 >= focusList.Count)
                    {
                        if (canLoop)
                            MoveTo(0);
                    }
                    else
                    {
                        MoveTo(curId + 1);
                    }
                }
            }
        }

        private int FindIndexByLabel(int label)
        {
            for (var i = 0; i < _labels.Count; i++)
                if (_labels[i] == label)
                    return i;

            return -1;
        }

        private void MoveToLabel(int label)
        {
            MoveTo(FindIndexByLabel(label));
        }

        public void MoveTo(int curId, bool force = false)
        {
            // Debug.Log($"move to try {curId}-{this.curId}, {_isFocused}");
            if (curId == this.curId && _isFocused && !force) return;
            if (curId < 0 || focusList.Count <= curId) return;
            FocusGroup?.ChangeFocusParent(this);
            // Debug.Log($"move to {curId}");
            if (_isFocused && this.curId < focusList.Count) FocusOff(this.curId);
            _isFocused = true;
            this.curId = curId;
            FocusChanged.Invoke(this.curId);
            FocusOn(this.curId);
        }

        private void FocusOn(int id)
        {
            if (isFocusSelect)
                focusList[id].SelectOn();
            else
                focusList[id].HoverOn();
        }

        private void FocusOff(int id)
        {
            if (isFocusSelect)
                focusList[id].SelectOff(true);
            else
                // Debug.Log("hover off");
                focusList[id].HoverOff(true);
        }
    }
}