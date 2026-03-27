using System.Collections;
using System.Collections.Generic;
using Apis.UI.Focus;
using Default;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_FocusSelector : UI_Base, IUI_Navigatable ,IUI_NavigationManager
{
    public List<ISelectableNavTarget> focusParents;

    [Tooltip("이 Selector가 활성화될 때 기본적으로 선택될 FocusParent의 인덱스")]
    public int initialFocusIndex = 0;

    private int _currentActiveIndex = -1; // 현재 활성화된 인덱스 추적
    private ISelectableNavTarget _currentActiveFocusParent;

    private bool _isInited;

    
    public override void Init()
    {
        base.Init();
        // 초기에는 모든 자식 FocusParent를 비활성화
        foreach (var fp in focusParents)
        {
            if (fp != null)
            {
                fp.NavigationManager = this;
                fp.OnDeselected();
                fp.OnNavigatedFrom();
            }
        }
    }
    /// <summary>
    /// 지정된 인덱스의 FocusParent를 활성화하고 나머지는 비활성화합니다.
    /// </summary>
    /// <param name="index">활성화할 FocusParent의 인덱스</param>
    public void SelectFocusByIndex(int index , bool focus)
    {
        if (focusParents == null || focusParents.Count == 0) return;
        if (index < 0 || index >= focusParents.Count)
        {
            // 유효하지 않은 인덱스면 현재 활성화된 것을 비활성화 (선택적 로직)
            if (_currentActiveFocusParent != null)
            {
                _currentActiveFocusParent.OnDeselected();
                _currentActiveFocusParent.OnNavigatedFrom();
                _currentActiveFocusParent = null;
            }

            _currentActiveIndex = -1;
            return;
        }

        // 이전에 활성화된 FocusParent가 있다면 비활성화 및 정리
        if (_currentActiveFocusParent != null && index != _currentActiveIndex)
        {
            _currentActiveFocusParent.OnDeselected();
            _currentActiveFocusParent.OnNavigatedFrom();
        }

        // 새로운 FocusParent 활성화
        _currentActiveFocusParent = focusParents[index];
        _currentActiveIndex = index;

        if (_currentActiveFocusParent != null)
        {
            _currentActiveFocusParent.OnSelected(focus);
            if (focus)
            {
                _currentActiveFocusParent.OnNavigatedTo();
            }
        }
    }

    public void OnNavigatedTo()
    {
        // 이 UI_FocusSelector 자체가 활성화될 때의 로직
        gameObject.SetActive(true); // Selector 자체의 GameObject 활성화 (필요하다면)

        NavigationManager?.SetCurrentNavigatable(this);

        // 기본 인덱스 또는 마지막으로 활성화됐던 인덱스로 자식 FocusParent 활성화
        // 여기서는 initialFocusIndex를 사용
        
        SelectFocusByIndex(_currentActiveIndex >= 0 ? _currentActiveIndex : initialFocusIndex,true);
    }

    public void OnNavigatedFrom()
    {
        // 이 UI_FocusSelector 자체가 비활성화될 때의 로직
        if (_currentActiveFocusParent != null)
        {
            _currentActiveFocusParent.OnNavigatedFrom(); // 현재 활성화된 자식도 정리
        }
    }

    public void KeyControl()
    {
        // 모든 키 입력을 현재 활성화된 자식 FocusParent에게 전달
        if (_currentActiveFocusParent != null && _currentActiveFocusParent.gameObject.activeSelf)
        {
            _currentActiveFocusParent.KeyControl();
        }
    }

    public void GamePadControl()
    {
        if (_currentActiveFocusParent != null && _currentActiveFocusParent.gameObject.activeSelf)
        {
            _currentActiveFocusParent.GamePadControl();
        }
    }

    public IUI_NavigationManager NavigationManager { get; set; }
    public bool IsAtBoundary(NavigationDirection direction)
    {
        if (_currentActiveFocusParent == null) return false;
        
        return _currentActiveFocusParent.IsAtBoundary(direction);
    }

    public override void InitCheck()
    {
        if (isInit) return;
        base.InitCheck();
        foreach (var focusParent in focusParents)
        {
            focusParent.InitCheck(); // 각 자식 FocusParent도 초기화 보장
            focusParent.NavigationManager = this; // 자식의 NavigationManager를 UI_FocusSelector 자신으로 설정
            focusParent.OnDeselected();
            focusParent.OnNavigatedFrom();
        }
    }

    public void SetCurrentNavigatable(IUI_Navigatable childNavigatable)
    {
        if (childNavigatable is ISelectableNavTarget clickedChildFp && focusParents.Contains(clickedChildFp))
        {
            int clickedIndex = focusParents.IndexOf(clickedChildFp);

            if (_currentActiveIndex != clickedIndex || _currentActiveFocusParent != clickedChildFp)
            {
                SelectFocusByIndex(clickedIndex, false); 
            }

            NavigationManager?.SetCurrentNavigatable(this);
        }
    }
}