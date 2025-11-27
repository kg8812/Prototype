using Apis.UI;
using Default;
using Managers;
using TMPro;

namespace UI
{
    public class UI_SystemAlert : UI_Popup
    {
        public enum Buttons
        {
            AcceptButton
        }

        public enum Texts
        {
            Text
        }

        private UIAsset_Button btn;
        private bool isAccepted;

        public override void Init()
        {
            base.Init();

            Bind<UIAsset_Button>(typeof(Buttons));
            Bind<TextMeshProUGUI>(typeof(Texts));
            btn = Get<UIAsset_Button>((int)Buttons.AcceptButton);
            btn.OnClick.AddListener(Accept);
        }

        public override void TryActivated(bool force = false)
        {
            isAccepted = false;
            base.TryActivated(force);
        }

        public override void KeyControl()
        {
            btn.KeyControl();
        }

        public override void GamePadControl()
        {
            btn.GamePadControl();
        }

        private void Accept()
        {
            if (isAccepted) return;
            isAccepted = true;
            SystemManager.SystemAlertComplete();
        }

        public void SetText(string text)
        {
            Get<TextMeshProUGUI>((int)Texts.Text).text = text;
        }
    }
}