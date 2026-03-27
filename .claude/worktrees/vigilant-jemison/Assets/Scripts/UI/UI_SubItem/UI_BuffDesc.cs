using Default;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UI_BuffDesc : UI_Base
    {
        [HideInInspector] public TextMeshProUGUI descTmp;
        [HideInInspector] public Image image;
        [HideInInspector] public RectTransform rect;

        [ReadOnly] public UI_BuffIcon currentIcon;

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            descTmp = Get<TextMeshProUGUI>((int)Texts.DescText);
            image = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
        }

        public void TurnOff()
        {
            image.enabled = false;
            descTmp.enabled = false;
            currentIcon = null;
        }

        public void TurnOn()
        {
            image.enabled = true;
            descTmp.enabled = true;
        }

        private enum Texts
        {
            DescText
        }
    }
}