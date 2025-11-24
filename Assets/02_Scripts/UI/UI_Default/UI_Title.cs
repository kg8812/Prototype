using Apis;
using Apis.UI;
using Apis.UI.Focus;
using Default;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_Title: UI_Base, IController
    {
        [SerializeField] private FocusParent focusParent;

        [SerializeField] private GameObject Credit;


        enum UIButtons
        {
            StartBtn, SettingBtn, CreditBtn, ExitBtn 
        }
        Button startButton;
        Button settingButton;
        Button exitButton;
        //
        // readonly List<GameObject> arrows = new();
        bool isSceneMove;

        private bool isShowingCredit = false;

        public override void Init()
        {
            base.Init();
            UIManager.SetCanvas(this, UIType.Default);
            focusParent.InitCheck();
            Bind<UIAsset_Button>(typeof(UIButtons));
            //
            // foreach(GameObjects arrow in Enum.GetValues(typeof(GameObjects)))
            // {
            //     arrows.Add(Get<GameObject>((int)arrow));
            //     arrows[arrows.Count - 1].SetActive(false);
            // }
            isSceneMove = false;
            
            Get<UIAsset_Button>((int)UIButtons.StartBtn).OnClick.AddListener(() =>
            {
                if (!isSceneMove)
                {
                    isSceneMove = true;
                    UI_SaveSlot slot = GameManager.UI.CreateUI("UI_SaveSlot", UIType.Scene) as UI_SaveSlot;
                    void NotSceneMove()
                    {
                        isSceneMove = false;
                        slot?.OnDeactivated.RemoveListener(NotSceneMove);
                    }
                    slot?.OnDeactivated.AddListener(NotSceneMove);
                }
            });
            
            Get<UIAsset_Button>((int)UIButtons.SettingBtn).OnClick.AddListener(() =>
            {
                GameManager.UI.CreateUI("UI_Setting", UIType.Popup);
            });
            
            Get<UIAsset_Button>((int)UIButtons.CreditBtn).OnClick.AddListener(() =>
            {
                isShowingCredit = true;
                Credit.SetActive(true);
            });
            
            
            Get<UIAsset_Button>((int)UIButtons.ExitBtn).OnClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        public void KeyControl()
        {
            if (isShowingCredit)
            {
                if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Cancel)))
                {
                    CloseCredit();
                }
            }
            else
            {
                focusParent.KeyControl();
            }
        }

        public void GamePadControl()
        {
            if (isShowingCredit)
            {
                if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Cancel)))
                {
                    CloseCredit();
                }
            }
            else
            {
                focusParent.GamePadControl();
            }
        }

        public void CloseCredit()
        {
            isShowingCredit = false;
            // Get<UIAsset_Button>((int)UIButtons.CreditBtn).UnSelected();
            Credit.SetActive(false);
        }

        public override void TryActivated(bool force = false)
        {
            focusParent.FocusReset();
            base.TryActivated(force);
        }

        protected override void Activated()
        {
            base.Activated();
            GameManager.UI.RegisterUIController(this);
        }
        
        private void OnDestroy()
        {
            if(_activated && !GameManager.IsQuitting)
                GameManager.UI.RemoveController(this);
        }
    }
   
}