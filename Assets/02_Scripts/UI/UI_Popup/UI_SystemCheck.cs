using Apis.UI;
using Apis.UI.Focus;
using Default;
using Managers;
using TMPro;
using UnityEngine;

namespace Apis
{
    public class UI_SystemCheck : UI_Popup
    {
        [SerializeField] private FocusParent focusParent;
        enum Texts
        {
            SystemText
        }

        enum Buttons
        {
            YesBtn,
            NoBtn
        }

        public override void KeyControl()
        {
            if (InputManager.GetKeyDown(KeySettingManager.GetUIKeyCode(Define.UIKey.Cancel)))
            {
                ChooseNo();
            }
            else
            {
                focusParent.KeyControl();
            }
        }

        public override void GamePadControl()
        {
            base.GamePadControl();
            
            if (InputManager.GetButtonDown(KeySettingManager.GetUIButton(Define.UIKey.Cancel)))
            {
                ChooseNo();
            }
            else
            {
                focusParent.GamePadControl();
            }
        }

        private bool choosed;

        public override void Init()
        {
            base.Init();

            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<UIAsset_Button>(typeof(Buttons));
            // Bind<Image>(typeof(HoverImgs));


            Get<UIAsset_Button>((int)Buttons.YesBtn).OnClick.AddListener(ChooseYes);
            Get<UIAsset_Button>((int)Buttons.NoBtn).OnClick.AddListener(ChooseNo);

            focusParent.InitCheck();
            
        }

        public override void TryActivated(bool force = false)
        {
            choosed = false;
            focusParent.FocusReset();
            base.TryActivated(force);
        }

        public void SetText(string text)
        {
            GetText((int)Texts.SystemText).text = text;
        }

        private void Choose(bool isYes)
        {
            if (!choosed)
            {
                choosed = true;
                SystemManager.SystemCheckComplete(isYes);
            }
        }


        private void ChooseYes()
        {
            Choose(true);
        }

        private void ChooseNo()
        {
            Choose(false);
        }
    }
}