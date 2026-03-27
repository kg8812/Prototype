using TMPro;
using UISpaces;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UI_SideAlert: UI_Main
    {
        public bool isMove = false;
        enum Texts
        {
            Title, Content
        }

        enum Imgs
        {
            Icon
        }

        public override void Init()
        {
            base.Init();

            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Image>(typeof(Imgs));
        }

        public void Alert(int iconInd, string title, string content)
        {
            // Get<Image>((int)Imgs.Icon).sprite = 
            GetText((int)Texts.Title).text = title;
            GetText((int)Texts.Content).text = content;
            ShowAlert();
        }

        private void ShowAlert()
        {
            
        }
    }
}