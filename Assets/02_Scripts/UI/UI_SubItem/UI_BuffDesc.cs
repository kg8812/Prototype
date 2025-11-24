using Default;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_BuffDesc : UI_Base
    {
        enum Texts
        {
            DescText,
        }
        [HideInInspector] public TextMeshProUGUI descTmp;
        [HideInInspector] public Image image;
        [HideInInspector] public RectTransform rect;
        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            descTmp = Get<TextMeshProUGUI>((int)Texts.DescText);
            image = GetComponent<Image>();
            rect = GetComponent<RectTransform>();
        }

        [ReadOnly] public UI_BuffIcon currentIcon;
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
    }
}