using Apis.Managers;
using Apis.UI;
using Apis.UI.Focus;
using UI;
using DG.Tweening;
using Default;
using Save.Schema;
using UnityEngine;

namespace Apis
{
    public class UI_Intro: UI_Base, IController
    {
        private bool choosed;
        [SerializeField] private float logoShowDuration = 3f;

        [HideInInspector] public IntroManager introManager;

        [SerializeField] private FocusParent focusParent;

        enum GameObjects
        {
            LanguageSelection
        }

        enum UIBtns
        {
            KoreanBtn, EnglishBtn
        }
        
        
        public void KeyControl()
        {
            if (choosed) return;
            focusParent.KeyControl();
        }

        public void GamePadControl()
        {
            if (choosed) return;
            focusParent.GamePadControl();
        }

        public override void Init()
        {
            base.Init();
            focusParent.InitCheck();
            UIManager.SetCanvas(this, UIType.Default);
            Bind<GameObject>(typeof(GameObjects));
            Bind<UIAsset_Button>(typeof(UIBtns));
            
            Get<UIAsset_Button>((int)UIBtns.KoreanBtn).OnClick.AddListener(() =>
            {
                SelectLanguage(LanguageType.Korean);
            });
            Get<UIAsset_Button>((int)UIBtns.EnglishBtn).OnClick.AddListener(() =>
            {
                SelectLanguage(LanguageType.English);
            });
        }

        private void SelectLanguage(LanguageType languageType)
        {
            LanguageManager.ChangedLanguageType(languageType);
            FadeManager.instance.Fading(ShowLogo);
        }

        private void ShowLogo()
        {
            if (DataAccess.GameData.Data.IsFirstGame)
            {
                DataAccess.GameData.Data.IsFirstGame = false;
                DataAccess.GameData.Save();
            }
            
            Get<GameObject>((int)GameObjects.LanguageSelection).SetActive(false);
            ShowTitle();
        }

        private void ShowTitle()
        {
            DOVirtual.DelayedCall(logoShowDuration, () =>
            {
                introManager.StartGame();
            });
        }

        public override void TryActivated(bool force = false)
        {
            
            if (DataAccess.GameData.Data.IsFirstGame)
            {
                choosed = false;
                focusParent.MoveTo((int)LanguageManager.LanguageType);
                GameManager.UI.RegisterUIController(this);
            }
            else
            {
                choosed = true;
                ShowLogo();
            }
            base.TryActivated(force);
        }
        
        private void OnDestroy()
        {
            if(_activated)
                GameManager.UI.RemoveController(this);
        }
    }
}