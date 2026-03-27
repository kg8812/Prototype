using System.Collections.Generic;
using Apis.UI.Focus;
using UnityEngine;

// 네비게이션 방향 Enum은 그대로 사용
public enum NavigationDirection
{
    Up,
    Down,
    Left,
    Right
}

// Unity 인스펙터에서 규칙을 쉽게 편집할 수 있도록 Serializable Struct를 생성
[System.Serializable]
public struct NavigationRule
{
    // 중요: 인터페이스는 인스펙터에 노출되지 않으므로, GameObject로 받은 뒤 코드로 변환
    public MonoBehaviour origin; 
    public NavigationDirection direction;
    public MonoBehaviour destination;
}

public interface IUI_Navigatable : IController
{
    // 이 UI가 활성화/비활성화 될 때 호출될 함수
    void OnNavigatedTo();
    void OnNavigatedFrom();

    // 마우스 클릭 등으로 자신이 활성화되었음을 매니저에게 보고할 수 있어야 함
    IUI_NavigationManager NavigationManager { get; set; }
    
    // 유니티 컴포넌트이므로 gameObject에 접근 가능해야 함
    GameObject gameObject { get; }
    bool IsAtBoundary(NavigationDirection direction);
    void InitCheck();
}
public interface IUI_NavigationManager
{
    void SetCurrentNavigatable(IUI_Navigatable newCurrent);
}
// ui 내부에서 키입력으로 focus 여러 곳 옮겨다니게 하기위한 스크립트입니다.
public class UI_NavigationController : MonoBehaviour, IController , IUI_NavigationManager
{
    [Header("네비게이션 설정")] [Tooltip("최초로 포커스를 받을 UI 그룹")]
    public MonoBehaviour initialFocus;

    [Tooltip("여기에 네비게이션 규칙을 모두 정의합니다.")] public List<NavigationRule> navigationRules;

    // 내부적으로 사용할 네비게이션 맵
    private Dictionary<IUI_Navigatable, Dictionary<NavigationDirection, IUI_Navigatable>> _navigationMap;
    private IUI_Navigatable _currentNavigatable;
    public IUI_Navigatable CurrentNavigatable => _currentNavigatable;
    private bool _isInitialized = false;

    public void SetCurrentNavigatable(IUI_Navigatable newCurrent)
    {
        ChangeFocus(newCurrent);
    }
    /// <summary>
    /// 외부에서 이 컨트롤러를 초기화할 때 호출합니다.
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized) return;

        // 리스트로 정의된 규칙들을 사용하기 편한 Dictionary 맵으로 변환
        _navigationMap = new Dictionary<IUI_Navigatable, Dictionary<NavigationDirection, IUI_Navigatable>>();
        var allParents = new HashSet<IUI_Navigatable>(); // 중복 등록 방지


        // MonoBehaviour로 받은 규칙들을 IUI_Navigatable 인터페이스 기반의 맵으로 변환
        foreach (var rule in navigationRules)
        {
            // GetComponent로 인터페이스를 가져옴
            IUI_Navigatable origin = rule.origin?.GetComponent<IUI_Navigatable>();
            IUI_Navigatable destination = rule.destination?.GetComponent<IUI_Navigatable>();
            
            if (origin != null && destination != null)
            {
                AddRule(origin, rule.direction, destination);
                origin.NavigationManager = this; // 네비게이션 매니저(자기 자신) 등록
                destination.NavigationManager = this;
                allParents.Add(origin);
            }
        }

        foreach (var uiNavigatable in allParents)
        {
            uiNavigatable.InitCheck();
        }
        _isInitialized = true;
    }

    // 2. IFocusGroup 인터페이스의 요구사항인 ChangeIUI_Navigatable 함수를 구현합니다.
    /// <summary>
    /// IUI_Navigatable로부터 "활성화 보고"를 받는 함수입니다.
    /// </summary>
    /// <param name="fp">새롭게 활성화된 IUI_Navigatable</param>
    public void ChangeIUI_Navigatable(IUI_Navigatable fp)
    {
        // 보고를 받으면, 관제탑의 현재 포커스 상태를 즉시 업데이트합니다.
        _currentNavigatable = fp;
    }
    
    /// <summary>
    /// 이 네비게이션 컨트롤러를 활성화합니다. (탭이 열릴 때 등)
    /// </summary>
    public void Activate()
    {
        if (!_isInitialized) Initialize();

        _isChangingFocus = false;
        IUI_Navigatable initial = initialFocus?.GetComponent<IUI_Navigatable>();
        
        if (initial != null)
        {
            ChangeFocus(initial);
        }
    }

    /// <summary>
    /// 이 네비게이션 컨트롤러를 비활성화합니다. (탭이 닫힐 때 등)
    /// </summary>
    public void Deactivate()
    {
        _currentNavigatable?.OnNavigatedFrom();
        _currentNavigatable = null;
    }


    /// <summary>
    /// 키 입력을 처리하는 네비게이션 엔진
    /// </summary>
    public void KeyControl()
    {
        if (_currentNavigatable == null)
        {
            Debug.LogWarning("[NavCtrl] KeyControl called but _currentNavigatable is null.");
            return;
        }

        // 각 방향키에 대해 실제 Unity 입력 상태를 먼저 확인
        KeyCode upKey = KeySettingManager.GetUIKeyCode(Define.UIKey.Up);
        if (Input.GetKeyDown(upKey)) // InputManager가 아닌 Unity의 Input 사용
        {
            if (TryNavigate(NavigationDirection.Up))
            {
                InputManager.GetKeyDown(upKey); // 그룹 네비게이션이 성공했으므로 InputManager에 '소비됨'으로 등록
                return; // 그룹 네비게이션이 발생했으므로 여기서 종료
            }
        }

        KeyCode downKey = KeySettingManager.GetUIKeyCode(Define.UIKey.Down);
        if (Input.GetKeyDown(downKey)) // InputManager가 아닌 Unity의 Input 사용
        {
            if (TryNavigate(NavigationDirection.Down))
            {
                InputManager.GetKeyDown(downKey); // 그룹 네비게이션이 성공했으므로 InputManager에 '소비됨'으로 등록
                return; 
            }
        }

        KeyCode leftKey = KeySettingManager.GetUIKeyCode(Define.UIKey.Left);
        if (Input.GetKeyDown(leftKey)) // InputManager가 아닌 Unity의 Input 사용
        {
            if (TryNavigate(NavigationDirection.Left))
            {
                InputManager.GetKeyDown(leftKey); // 그룹 네비게이션이 성공했으므로 InputManager에 '소비됨'으로 등록
                return;
            }
        }

        KeyCode rightKey = KeySettingManager.GetUIKeyCode(Define.UIKey.Right);
        if (Input.GetKeyDown(rightKey)) // InputManager가 아닌 Unity의 Input 사용
        {
            if (TryNavigate(NavigationDirection.Right))
            {
                InputManager.GetKeyDown(rightKey); // 그룹 네비게이션이 성공했으므로 InputManager에 '소비됨'으로 등록
                return;
            }
        }
        
        _currentNavigatable.KeyControl();
    }

    public void GamePadControl()
    {
        if (_currentNavigatable == null) return;

        // 각 방향키에 대해 네비게이션 시도
        if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Up)))
            if (TryNavigate(NavigationDirection.Up))
                return;

        if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Down)))
            if (TryNavigate(NavigationDirection.Down))
                return;

        if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Left)))
            if (TryNavigate(NavigationDirection.Left))
                return;

        if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Right)))
            if (TryNavigate(NavigationDirection.Right))
                return;

        // 네비게이션이 일어나지 않았다면, 현재 그룹이 입력을 처리
        _currentNavigatable.GamePadControl();
    }

    public bool TryNavigate(NavigationDirection direction)
    {
        if (_currentNavigatable.IsAtBoundary(direction) &&
            _navigationMap.TryGetValue(_currentNavigatable, out var destinations) &&
            destinations.TryGetValue(direction, out var nextFocus))
        {
            ChangeFocus(nextFocus);
            return true;
        }

        return false;
    }
    private bool _isChangingFocus;

    private void ChangeFocus(IUI_Navigatable next)
    {
        if (_isChangingFocus) return;
        if (next == null) return;

        _isChangingFocus = true;

        if (_currentNavigatable != next)
        {
            _currentNavigatable?.OnNavigatedFrom();
            _currentNavigatable = next;
        }

        _currentNavigatable?.OnNavigatedTo();

        _isChangingFocus = false;
    }
    
    private void AddRule(IUI_Navigatable origin, NavigationDirection direction, IUI_Navigatable destination)
    {
        if (origin == null || destination == null) return;
        if (!_navigationMap.ContainsKey(origin))
        {
            _navigationMap[origin] = new Dictionary<NavigationDirection, IUI_Navigatable>();
        }

        _navigationMap[origin][direction] = destination;
    }
}