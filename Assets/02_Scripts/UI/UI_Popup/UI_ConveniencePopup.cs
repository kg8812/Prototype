using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_ConveniencePopup : UI_Popup
    {

        private readonly Color _enoughColor = Color.white;
        private readonly Color _lackColor = new Color(178, 50, 50);
        enum Texts
        {
            NameText,Description,CostText, CostTitle, CostWarning
        }

        enum Buttons
        {
            Buy,Close
        }
        private TextMeshProUGUI nameText;
        private TextMeshProUGUI description;
        // private TextMeshProUGUI soulText;
        private TextMeshProUGUI costText;
        private Button buyButton;
        private Button closeButton;
        
        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Button>(typeof(Buttons));
            nameText = GetText((int)Texts.NameText);
            description = GetText((int)Texts.Description);
            costText = GetText((int)Texts.CostText);
            buyButton = GetButton((int)Buttons.Buy);
            closeButton = GetButton((int)Buttons.Close);
            closeButton.onClick.AddListener(() => GameManager.UI.CloseUI(this));
        }

        private UI_Convenience conv;
        
        public void Init(UI_Convenience conv)
        {
            this.conv = conv;
        }

        public void Set(UI_ConvenienceSub sub)
        {
            nameText.text = sub.Data.name;
            description.text = sub.Data.desc;
            costText.text = $"{GameManager.instance.LobbySoul.ToString()}/{sub.Data.priceWarmth.ToString()}";
            // soulText.text = ;

            if (GameManager.instance.LobbySoul >= sub.Data.priceWarmth)
            {
                costText.color = _enoughColor;
                GetText((int)Texts.CostTitle).color = _enoughColor;
                GetText((int)Texts.CostWarning).enabled = false;
                buyButton.gameObject.SetActive(true);
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() =>
                {
                    GameManager.instance.LobbySoul -= sub.Data.priceWarmth;
                    sub.Purchase();
                    GameManager.UI.CloseUI(this);
                    conv.CreateList();
                    conv.SetList();
                });
            }
            else
            {
                costText.color = _lackColor;
                GetText((int)Texts.CostTitle).color = _lackColor;
                GetText((int)Texts.CostWarning).enabled = true;
                buyButton.gameObject.SetActive(false);
            }
        }

        public override void TryDeactivated(bool force = false)
        {
            base.TryDeactivated(force);
            GameManager.UiController = conv;
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            conv.popup = null;
            
        }
    }
}