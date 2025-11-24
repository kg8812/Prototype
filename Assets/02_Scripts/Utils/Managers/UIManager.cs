using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apis;
using Apis.UI;
using Default;
using GameStateSpace;
using Managers;
using UI;
using UISpaces;
using UnityEngine;
using GUtil = Default.Utils;
using Object = UnityEngine.Object;


namespace Apis
{
    public enum UIType
    {
        Default,
        Ingame,
        Main,
        Scene,
        Popup,
        Hover,
        SubItem
    }

    public class UIManager
    {
        private const int DefaultOrder = 0;
        private const int MainOrder = 1;
        private const int InGameInitOrder = 0;
        private const int AddInitOrder = 50;
        private const int HoverOrder = 100;
        
        private int _addOrder = AddInitOrder;
        private int _inGameOrder = InGameInitOrder;

        // main list는 activation 과 영향 x
        private List<UI_Main> _mainList;
        private Stack<UI_Base> _addtionalUi; // 오브젝트 말고 컴포넌트를 담음. 팝업 캔버스 UI 들을 담는다.
        private List<UI_Ingame> _inGameList;

        private List<IController> _uiControllerList;
        
        private UI_Scene _sceneUI; // 현재의 고정 캔버스 UI
        private UI_Base _hoverUI; // 현재의 마우스 호버 ui
        
        private bool _mainUiToggle;

        private Guid _uiGuid;


        public void Init()
        {
            _mainList = new();
            _addtionalUi = new();
            _inGameList = new();
            _uiControllerList = new();

            _mainUiToggle = GameManager.Scene.CurSceneData.isPlayerMustExist;
        }

        
        private AddressablePooling _pool = null;
        
        private AddressablePooling Pool
        {
            get
            {
                _pool ??= new("UI_Pool");

                return _pool;
            }
        }
        
        
        private Transform _root;

        private Transform Root
        {
            get
            {
                if (!ReferenceEquals(_root, null)) return _root;

                _root = GameObject.Find("@UI_Root")?.transform;
                if (ReferenceEquals(_root, null))
                {
                    GameObject obj = new GameObject { name = "@UI_Root" };
                    obj.transform.position = new Vector3(0, -40, 0);
                    obj.isStatic = true;
                    Object.DontDestroyOnLoad(obj);
                    _root = obj.transform;
                }
                return _root;
            }
        }


        public void ToggleMainUI(bool isOn)
        {
            _mainUiToggle = isOn;

            foreach (var uiMain in _mainList)
            {
                MainUISetActive(uiMain);
            }
        }

        private void MainUISetActive(UI_Main mainUI)
        {
            // 같으면 
            mainUI.MainUIShow = _mainUiToggle;
        }

        // ui init에서 호출.
        public static void SetCanvas(UI_Base baseUI, UIType uiType)
        {
            baseUI._canv.renderMode = RenderMode.ScreenSpaceCamera;
            baseUI._canv.worldCamera = CameraManager.instance.UICam;
            baseUI._canv.overrideSorting = true; // 캔버스 안에 캔버스 중첩 경우 (부모 캔버스가 어떤 값을 가지던 나는 내 오더값을 가지려 할때)

            baseUI._canv.sortingLayerID = uiType == UIType.Ingame ? SortingLayers.InGameUI : SortingLayers.UI;
        }

        public UI_Base CreateUI(string prefabName, UIType uiType, bool hideBaseUI = false, bool withoutActivation = false)
        {
            // 오브젝트를 가져오기 전에 ReShow를 하니까 오브젝트가 꺼진 상태에서 코루틴 호출해서 에러가 났습니다.
            // 그래서 Get 함수 호출을 아래쪽에서 여기로 옮겼습니다.
            
            // 메인과 같은 경우는 고유성이 존재하기 때문
            if (uiType == UIType.Main)
            {
                foreach (var mainUI in _mainList.ToList())
                {
                    if (mainUI.name == prefabName && mainUI.IsActivated)
                    {
                        mainUI.ReShow();
                        return mainUI;
                    }
                }
            }
            
            GameObject go = Pool.Get($"Prefabs/UI/{uiType}/{prefabName}");
            UI_Base uiComponent = GUtil.GetOrAddComponent<UI_Base>(go);

            return UIInitSetting(uiComponent, uiType, withoutActivation);
        }

        public UI_Base UIInitSetting(UI_Base uiComponent, UIType uiType, bool withoutActivation = false)
        {
            if (!uiComponent.IsInit)
            {
                uiComponent.Init();
            }

            if(uiType != UIType.Default)
                uiComponent.transform.SetParent(Root.transform);

            if (uiType == UIType.Main && uiComponent is UI_Main mainUi)
            {
                _mainList.Add(mainUi);
                MainUISetActive(mainUi);
            }

            if (!withoutActivation)
                uiComponent.TryActivated();


            return uiComponent;
        }

        public int RegisterUI(UI_Base uiComponent)
        {
            int order = -1;
            switch (uiComponent)
            {
                case UI_Main uiMain:
                    uiMain._canv.sortingOrder = MainOrder;
                    order = MainOrder;
                    break;
                
                case UI_Scene:
                case UI_Popup:
                    _addtionalUi.Push(uiComponent);
                    uiComponent._canv.sortingOrder = _addOrder;
                    order = _addOrder++;
                    break;
                
                case UI_Ingame uiInGame:
                    _inGameList.Add(uiInGame);
                    uiInGame._canv.sortingOrder = _inGameOrder;
                    order = _inGameOrder++;
                    break;
                
                case UI_Hover uiHover:
                    _hoverUI = uiHover;
                    uiHover._canv.sortingOrder = HoverOrder;
                    order = HoverOrder;
                    break;
                
                default:
                    uiComponent._canv.sortingOrder = DefaultOrder;
                    order = DefaultOrder;
                    break;
            }
            return order;
            
        }

        public void RegisterUIController(IController controller)
        {
            GameManager.UiController = controller;
            if (_uiControllerList.Count == 0)
            {
                _uiGuid = GameManager.instance.TryOnGameState(GameStateType.InteractionState);
            }
                
            _uiControllerList.Add(controller);
        }
        
        public UI_Base MakeSubItem(string prefabName, Transform parent)
        {
            GameObject go = Pool.Get($"Prefabs/UI/SubItem/{prefabName}");
            
            UI_Base subItem = GUtil.GetOrAddComponent<UI_Base>(go);
            subItem.isChild = true;
            
            if (!ReferenceEquals(parent, null))
            {
                go.transform.SetParent(parent);
                if (parent.TryGetComponent(out UI_Base parentBase))
                {
                    parentBase.subItems.Add(subItem);
                }
            }
            if (!subItem.IsInit)
            {
                subItem.Init();
            }
            
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            subItem.TryActivated();

            return subItem;
        }

        public T MakeUI<T>(string prefabName, Transform parent) where T : Component
        {
            GameObject go = Pool.Get($"Prefabs/UI/SubItem/{prefabName}");
            if (!ReferenceEquals(parent, null))
            {
                go.transform.SetParent(parent);
            }
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            return go.GetOrAddComponent<T>();
        }

        
        
        public void CloseUI(UI_Base baseUi, bool force = false)
        {
            if (!baseUi) return;

            switch (baseUi)
            {
                case UI_Main mainUi:
                    break;
                
                case UI_Scene:
                case UI_Popup:
                    if (_addtionalUi.Count > 0)
                    {
                        if (ReferenceEquals(_addtionalUi.Peek(), baseUi))
                        {
                            _addtionalUi.Pop();
                            if (_addtionalUi.Count == 0)
                            {
                                _addOrder = AddInitOrder;
                            }
                        }
                    }
                    break;
                
                case UI_Ingame ingameUi:
                    if (_inGameList.Count > 0 && _inGameList.Contains(ingameUi))
                    {
                        _inGameList.Remove(ingameUi);
                        if (_inGameList.Count == 0)
                        {
                            _inGameOrder = InGameInitOrder;
                        }
                    }
                    break;
            }

            if(baseUi is IController controller)
                RemoveController(controller);
            
            baseUi.TryDeactivated(force);
        }

        public void RemoveController(IController baseUi)
        {
            if (_uiControllerList.Count == 0) return;
            for (int i = _uiControllerList.Count-1; i >= 0; i--)
            {
                if (ReferenceEquals(baseUi, _uiControllerList[i]))
                {
                    _uiControllerList.Remove(_uiControllerList[i]);
                    if (_uiControllerList.Count == 0)
                    {
                        GameManager.UiController = null;
                        GameManager.instance.TryOffGameState(GameStateType.InteractionState, _uiGuid);
                    }
                    else if(i == _uiControllerList.Count)
                    {
                        GameManager.UiController = _uiControllerList[i-1];
                    }
                }
            }
            
        }
        
        public void ReturnUI(UI_Base ui)
        {
            Pool.Return(ui.gameObject);
        }

        public void ReturnUI(GameObject ui)
        {
            Pool.Return(ui);
        }
        
        // ui element section
        // public T CreateUIElement<T>(string prefabName, Transform parent) where T : UIElement
        // {
        //     GameObject go = Pool.Get($"Prefabs/UI/Element/{prefabName}");
        //     // Debug.Log("create ui ele");
        //     if (!ReferenceEquals(parent, null))
        //     {
        //         // Debug.Log($"set create ui ele parent {parent.transform.name}");
        //         go.transform.SetParent(parent);
        //     }
        //     go.transform.localScale = Vector3.one;
        //     go.transform.localPosition = Vector3.zero;
        //
        //     return go.GetOrAddComponent<T>();
        // }
        //
        // public void ReturnUIElement(UIElement el)
        // {
        //     Pool.Return(el.gameObject);
        // }
    }
}