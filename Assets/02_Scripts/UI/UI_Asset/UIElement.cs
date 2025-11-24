using System;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Apis.UI
{
    [Flags]
    public enum UIElementState
    {
        Default = 1 << 0,
        Hover = 1 << 1,
        Select = 1 << 2,
        Disable = 1 << 3,
        Pressed = 1 << 4
    }

    public delegate void FocusToggle();

    public abstract class UIElement : UI_Base, IController, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Tooltip("true로 설정하면 클릭(Select)해야만 포커스를 받는 방식으로, false이면 마우스 호버만으로도 포커스를 받는 방식으로 동작을 변경할 수 있습니다.")]
        public bool isFocusSelect; 
        [SerializeField] private UIElementState InitState = UIElementState.Default;

        [HideInInspector] public RectTransform rectTransform;

        // 정지된 상태. 현재 상태에서 어떠한 변화도 허락하지 않음.
        [HideInInspector] public bool isFrozen;
        
        // 선택된지 여부
        public bool IsSelected { get; protected set; }
        
        // 마우스 hover 여부
        // hover state와 관계없이 마우스가 올라가 있냐?만 나타낸다
        public bool IsHovered { get; protected set; }

        private bool _isFocused;
        public bool IsFocused
        {
            get => _isFocused;
            protected set
            {
                if (_isFocused == value) return;
                _isFocused = value;
                if(_isFocused)
                    FocusOn?.Invoke();
                else
                    FocusOff?.Invoke();
            }
        }
        
        // 비활성화 여부
        public bool IsDisable { get; protected set; }

        [ReadOnly] [ShowInInspector] private UIElementState _elState;

        public UIElementState ElState
        {
            get => _elState;
            set
            {
                if (isFrozen) return;
                WillStateChange?.Invoke(_elState);
                _elState = value;
                // Debug.LogError($"set el state to {_elState}");
                StateChanged?.Invoke(_elState);
            }
        }

        [HideInInspector] public bool focusOffByParent;
        

        [HideInInspector] public Action<UIElementState> WillStateChange;
        [HideInInspector] public Action<UIElementState> StateChanged;
        
        public FocusToggle FocusOn;
        public FocusToggle FocusOff;


        public override void Init()
        {
            base.Init();
            rectTransform = GetComponent<RectTransform>();

            ElState = InitState;
            if (EqualSt(ElState, UIElementState.Disable))
            {
                IsDisable = true;
            }
            WillStateChange += WillElStateChange;
            StateChanged += ElStateChanged;
        }

        public static bool EqualSt(UIElementState from, UIElementState to)
        {
            return (from & to) == to;
        }

        protected virtual void Start()
        {
            InitCheck();
        }

        public void UpdateFocusType()
        {
            if (isFrozen) return;
            if (!isFocusSelect && EqualSt(ElState, UIElementState.Select))
            {
                ElState = UIElementState.Hover;
            }
            else if(isFocusSelect && EqualSt(ElState, UIElementState.Hover))
            {
                ElState = UIElementState.Select;
            }
        }

        protected virtual void WillElStateChange(UIElementState elState)
        {
            
        }

        protected virtual void ElStateChanged(UIElementState elState)
        {
            
        }

        public virtual void FrozenToggle(bool isOn)
        {
            isFrozen = isOn;
        }

        #region UIEventListener
        
        /**
         * Disable > Select > Press > Hover 순으로 우선순위
         */

        public virtual void SelectOn()
        {
            if ((ElState & (UIElementState.Hover | UIElementState.Default | UIElementState.Pressed)) != 0)
            {
                IsSelected = true;
                ElState = UIElementState.Select;
                IsFocused = true;
            }
        }

        public virtual void SelectOff(bool force = false)
        {
            if ((!isFocusSelect || !focusOffByParent || force) && EqualSt(ElState, UIElementState.Select))
            {
                IsSelected = false;
                ElState = IsHovered ? UIElementState.Hover : UIElementState.Default;
                if (!IsHovered || isFocusSelect)
                    IsFocused = false;
            }
        }

        
        // disable 상태 제외하고 무조건 pressed 상태로 진입.
        public void PressOn()
        {
            if (isFrozen) return;
            if (!IsDisable)
            {
                ElState = UIElementState.Pressed;
            }
        }

        // Press -> Select는 SelectOn에서 처리
        // 해당 함수는 Press -> Hover, Default만 처리
        public void PressOff()
        {
            if (isFrozen) return;
            if (!IsDisable && EqualSt(ElState, UIElementState.Pressed))
            {
                ElState = IsHovered ? UIElementState.Hover : UIElementState.Default;
                if (!IsHovered)
                    IsFocused = false;
            }
        }
        
        // 마우스 hover하면 출력.
        // default -> hover만 취급. (select -> hover는 안됨)
        public void HoverOn()
        {
            if (isFrozen) return;
            if (!IsDisable &&EqualSt(ElState, UIElementState.Default))
            {
                ElState = UIElementState.Hover;
                if (!isFocusSelect)
                    IsFocused = true;
            }
        }
        
        // 마우스 hover 나가면 출력.
        // 
        public void HoverOff(bool force = false)
        {
            if (isFrozen) return;
            if (!IsDisable && (isFocusSelect || !focusOffByParent || force) && (ElState & (UIElementState.Hover | UIElementState.Pressed)) != 0)
            {
                ElState = UIElementState.Default;
                IsFocused = false;
            }
        }
        
        
        public void DisableOn()
        {
            if (isFrozen) return;
            IsDisable = true;
            ElState = UIElementState.Disable;
            IsFocused = false;
        }

        public void DisableOff()
        {
            if (isFrozen) return;
            IsDisable = false;
            ElState = IsSelected ? UIElementState.Select : (IsHovered ? UIElementState.Hover : UIElementState.Default);
            if (IsSelected || (IsHovered && !isFocusSelect))
                IsFocused = true;
        }


        #endregion
        
        

        public virtual void KeyControl()
        {
            
        }

        public virtual void GamePadControl()
        {
            
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            SelectOn();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PressOn();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            PressOff();
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            IsHovered = true;
            HoverOn();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            IsHovered = false;
            HoverOff();
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            
        }
    }
}