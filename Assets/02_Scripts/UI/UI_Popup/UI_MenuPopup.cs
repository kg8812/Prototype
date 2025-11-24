using System;
using Default;
using Apis.Managers;
using Apis.UI;
using Apis.UI.Focus;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class UI_MenuPopup : UI_Popup
    {
        private enum Buttons
        {
            SettingBtn,TipBtn,TitleBtn
        }

        [SerializeField] private FocusParent focusParent;

        private Guid _pauseGuid;

        public override void Init()
        {
            base.Init();
            Bind<UIAsset_Button>(typeof(Buttons));

            Get<UIAsset_Button>((int)Buttons.SettingBtn).OnClick.AddListener(() =>
            {
                GameManager.UI.CreateUI("UI_Setting", Apis.UIType.Popup);
            });
            Get<UIAsset_Button>((int)Buttons.TipBtn).OnClick.AddListener(() =>
            {
                CloseOwn();
                GameManager.UI.CreateUI("UI_TipList", Apis.UIType.Scene);
            });
            Get<UIAsset_Button>((int)Buttons.TitleBtn).OnClick.AddListener(() =>
            {
                string str = LanguageManager.Str(SceneManager.GetActiveScene().name != Define.SceneNames.LobbySceneName ? 1010422 : 1010421);
                SystemManager.SystemCheck(str, (isYes) =>
                {
                    if (isYes)
                    {
                        FadeManager.instance.fadeIn.AddListener(CloseOwn);
                        GameManager.Scene.SceneLoad(Define.SceneNames.TitleSceneName);
                        GameManager.Sound.StopAllSounds();
                    }
                });
            });
            
            focusParent.InitCheck();
        }

        public override void TryActivated(bool force = false)
        {
            focusParent.FocusReset();
            base.TryActivated(force);
        }

        protected override void Activated()
        {
            base.Activated();
            _pauseGuid = GameManager.instance.RegisterPause();
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            GameManager.instance.RemovePause(_pauseGuid);
        }


        public override void KeyControl()
        {
            base.KeyControl();
            focusParent?.KeyControl();
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            focusParent?.GamePadControl();
        }
    }
}