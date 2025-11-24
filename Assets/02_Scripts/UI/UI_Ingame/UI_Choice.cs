using System.Collections.Generic;
using Apis.UI;
using Apis.UI.Focus;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public struct MenuItem
    {
        public string text;
        public UnityAction action;

        public MenuItem(string text, UnityAction action)
        {
            this.text = text;
            this.action = action;
        }
    }
    public class UI_Choice : UI_Ingame, IController
    {
        enum Texts
        {
            Text1, Text2, Text3, Text4
        }

        enum Btns
        {
            Menu1, Menu2, Menu3, Menu4, CancelButton
        }

        enum RectTransforms
        {
            Content, HoverImg
        }

        private const int FullMenuCnt = 4;
        private const float ItemHeight = 70;
        private readonly Vector2 _offset = new (0, 200);

        private int _itemCnt;
        private int _curHoveredItem;
        private List<UnityAction> _actions;
        private RectTransform _rect;

        [SerializeField] private FocusParent focusParent;

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<UIAsset_Button>(typeof(Btns));
            Bind<RectTransform>(typeof(RectTransforms));
            _rect = Get<RectTransform>((int)RectTransforms.Content);
            _actions = new();
        }

        public void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Cancel)))
            {
                GameManager.UI.CloseUI(this);
                return;
            }

            focusParent?.KeyControl();
        }

        public void GamePadControl()
        {
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Cancel)))
            {
                GameManager.UI.CloseUI(this);
                return;
            }
            focusParent?.GamePadControl();

        }

        public void SetMenu(List<MenuItem> menuItems)
        {
            _itemCnt = menuItems.Count;
            
            focusParent.Reset(true);
            // _actions.Clear();
            for (int i = 0; i < FullMenuCnt; i++)
            {
                bool isShow = i < _itemCnt;
                if (isShow)
                {
                    GetText(i).text = menuItems[i].text;
                    UnityAction action = menuItems[i].action;
                    Get<UIAsset_Button>(i).InitCheck();
                    Get<UIAsset_Button>(i).OnClick.RemoveAllListeners();
                    Get<UIAsset_Button>(i).OnClick.AddListener(() =>
                    {
                        CloseOwn();
                        action.Invoke();
                    });
                    focusParent.RegisterElement(Get<UIAsset_Button>(i));
                }
                Get<UIAsset_Button>(i).gameObject.SetActive(isShow);
            }
            focusParent.RegisterElement(Get<UIAsset_Button>(FullMenuCnt));

            Get<RectTransform>((int)RectTransforms.Content).sizeDelta =
                new Vector2(260f, 400 - ItemHeight * (FullMenuCnt - _itemCnt));
            focusParent.FocusReset();
        }

        
        public void SetTrans(Transform trans)
        {
            this.targetTrans = trans;
        }
        
        protected override void PositioningFollower()
        {
            _rect.anchoredPosition = calcPos + _offset;
        }

        public override void TryActivated(bool force = false)
        {
            base.TryActivated(force);
            GameManager.UI.RegisterUIController(this);
        }

    }
}