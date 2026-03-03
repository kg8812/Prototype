using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Apis;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Default
{ 
    public class UI_Base : SerializedMonoBehaviour
    {
        // alreadyCreated section
        [FormerlySerializedAs("isChild")] public bool isSubItem;
        public bool isAlreadyCreated;
        [ShowIf("isAlreadyCreated", true)] [SerializeField]
        private UIType showType;
        [ShowIf("isAlreadyCreated", true)] [SerializeField]
        private bool activatedWhenInit;
        
        protected Dictionary<Type, UnityEngine.Object[]> _object = new Dictionary<Type, UnityEngine.Object[]>();
        [HideInInspector] public readonly List<UI_Base> subItems = new();
        [HideInInspector] public Canvas _canv;
        [HideInInspector] public CanvasGroup _canvGroup;
        
        [HideInInspector] public UnityEvent OnActivated = new();
        [HideInInspector] public UnityEvent OnDeactivated = new();
        
        public bool isFadeIn;
        public bool isFadeOut;

        [Tooltip("자식에 스크롤을 사용하는 ui가 있으면 체크하세요. 켜질 때 스크롤을 초기화시킵니다.")][SerializeField] bool useScrolls;
        
        private bool _isParent;
        private bool _hasCanvas;

        private Sequence _fadeSeq;
        
        protected bool _activated;
        protected bool isInit = false;
        public bool IsInit => isInit;
        public bool IsActivated => _activated;

        private void Start()
        {
            if (isAlreadyCreated && !isSubItem)
            {
                GameManager.UI.UIInitSetting(this, showType, true);
                if (activatedWhenInit)
                {
                    TryActivated(true);
                }
            }
        }
        
        public virtual void TryActivated(bool force = false)
        {
            if (_isParent)
            {
                GameManager.UI.RegisterUI(this);
                if (isFadeIn && !force && _hasCanvas)
                {
                    FadeCanvas(true, Activated);
                }
                else
                {
                    Activated();
                }
            }
            subItems.ForEach(sub => sub.TryActivated(force));
        }

        protected virtual void Activated()
        {
            if (_activated) return;
            
            _activated = true;
            
            if(_isParent)
                _canvGroup.alpha = 1;
            if(_hasCanvas)
                _canv.enabled = true;
            
            
            subItems.ForEach(sub => sub.Activated());
            OnActivated.Invoke();
            
        }
        
        
        public virtual void TryDeactivated(bool force = false)
        {
            if (_isParent || !isAlreadyCreated || !isSubItem)
            {
                if (isFadeOut && !force && _hasCanvas)
                {
                    FadeCanvas(false, Deactivated);
                }
                else
                {
                    Deactivated();
                }
            }
            subItems.ForEach(sub => sub.TryDeactivated(force));
        }
        
        protected virtual void Deactivated()
        {
            _activated = false;
            if(_hasCanvas)
                _canv.enabled = false;
            subItems.ForEach(sub => sub.Deactivated());
            if (!isAlreadyCreated || !isSubItem)
            {
                GameManager.UI.ReturnUI(this);
            }
            OnDeactivated.Invoke();
        }

        public virtual void InitCheck()
        {
            if (isInit) return;
            Init();
        }

        public virtual void Init()
        {
            isInit = true;
            _canv = GetComponent<Canvas>();
            _canvGroup = GetComponent<CanvasGroup>();
            _hasCanvas = _canv != null;
            _isParent = _canvGroup != null;

            if (useScrolls)
            {
                OnActivated.AddListener(() =>
                {
                    ScrollRect[] scrolls = GetComponentsInChildren<ScrollRect>(true);
                    foreach (var scroll in scrolls)
                    {
                        scroll.verticalNormalizedPosition = 1;
                        scroll.horizontalNormalizedPosition = 1;
                    }
                });
            }
        }

        protected void Bind<T>(Type type) where T : UnityEngine.Object
        {
            string[] names = Enum.GetNames(type);
            UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
            
            _object.Add(typeof(T), objects);
            for (int i = 0; i < names.Length; i++)
            {              
                if (typeof(T) == typeof(GameObject))
                    objects[i] = Utils.FindChild(gameObject, names[i], true);
                else
                    objects[i] = Utils.FindChild<T>(gameObject, names[i], true);
                //if (objects[i] == null)
                //Debug.Log($"Failed to bind({names[i]})");
            }
            
        }
        protected T Get<T>(int idx) where T : UnityEngine.Object
        {
            _object.TryGetValue(typeof(T), out UnityEngine.Object[] objects);
            if (_object.TryGetValue(typeof(T), out objects) == false)
                return null;
            return objects[idx] as T;
        }
        protected TextMeshProUGUI GetText(int idx) { return Get<TextMeshProUGUI>(idx); }
        protected Button GetButton(int idx) { return Get<Button>(idx); }
        protected Image GetImage(int idx) { return Get<Image>(idx); }

        public static void AddUIEvent(GameObject go, Action<PointerEventData> action, Define.UIEvent type = Define.UIEvent.Click, UI_EventHandler evtPre = null)
        {
            UI_EventHandler evt;
            if (ReferenceEquals(evtPre, null))
            {
                evt = Utils.GetOrAddComponent<UI_EventHandler>(go);
            }
            else
            {
                evt = evtPre;
            }
            switch (type)
            {
                case Define.UIEvent.Click:
                    evt.OnClickHandler -= action;
                    evt.OnClickHandler += action;
                    break;
                case Define.UIEvent.BeginDrag:
                    evt.OnBeginDragHandler -= action;
                    evt.OnBeginDragHandler += action;
                    break;
                case Define.UIEvent.Drag:
                    evt.OnDragHandler -= action;
                    evt.OnDragHandler += action;
                    break;
                case Define.UIEvent.EndDrag:
                    evt.OnEndDragHandler -= action;
                    evt.OnEndDragHandler += action;
                    break;
                case Define.UIEvent.Drop:
                    evt.OnDropHandler -= action;
                    evt.OnDropHandler += action;
                    break;
                case Define.UIEvent.PointDown:
                    evt.OnPointerUpHandler -= action;
                    evt.OnPointerUpHandler += action;
                    break;
                case Define.UIEvent.PointUp:
                    evt.OnPointerUpHandler -= action;
                    evt.OnPointerUpHandler += action;
                    break;
                case Define.UIEvent.PointEnter:
                    evt.OnPointerEnterHandler -= action;
                    evt.OnPointerEnterHandler += action;
                    break;
                case Define.UIEvent.PointExit:
                    evt.OnPointerExitHandler -= action;
                    evt.OnPointerExitHandler += action;                   
                    break;
                case Define.UIEvent.PointStay:
                    evt.OnPointerStayHandler -= action;
                    evt.OnPointerStayHandler += action;
                    break;
            }
        }

        protected void AddSubItem(UI_Base subItem)
        {
            subItems.Add(subItem);
            
            if (!subItem.IsInit)
            {
                subItem.Init();
            }
            subItem.isSubItem = true;
        }
        protected void RemoveSubItem(UI_Base subitem)
        {
            subItems.Remove(subitem);
            subitem.Deactivated();
        }

        public void ToggleCanvas(bool isOn, bool force = false)
        {
            _fadeSeq?.Kill();
            if (isOn && isFadeIn && !force)
            {
                FadeCanvas(true);
            }else if (!isOn && isFadeOut && !force)
            {
                FadeCanvas(false);
            }
            else
            {
                _canvGroup.alpha = isOn ? 1 : 0;
                _canv.enabled = isOn;
            }
        }

        public void FadeCanvas(bool isOn, UnityAction completeAction = null)
        {
            _fadeSeq?.Kill();
            _fadeSeq = DOTween.Sequence()
                .OnStart(() =>
                {
                    _canvGroup.alpha = isOn ? 0 : 1;
                    _canv.enabled = true;
                }).SetUpdate(true)
                .Append(_canvGroup.DOFade(isOn ? 1 : 0, FormulaConfig.uiFadeInDuration).SetUpdate(true))
                .OnComplete(() =>
                {
                    _canv.enabled = isOn;
                    completeAction?.Invoke();
                });
        }
        

        public virtual void CloseOwn()
        {
            GameManager.UI.CloseUI(this);
        }

        public virtual void CloseOwnForce()
        {
            GameManager.UI.CloseUI(this, true);
        }
    }
}