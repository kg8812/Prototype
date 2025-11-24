using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_ItemInfo : UI_Hover
    {
        enum GameObjects
        {
            Background
        }

        enum Images
        {
            ItemImg
        }

        enum Texts
        {
            ItemName,
            ItemInfo
        }

        public override void Init()
        {
            base.Init();

            Bind<GameObject>(typeof(GameObjects));
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(Texts));
            _contentTrans = Get<GameObject>((int)GameObjects.Background).GetComponent<RectTransform>();
        }

        public void SetInfo(string imgPath, string itemName, string itemInfo)
        {
            Get<Image>((int)Images.ItemImg).sprite = ResourceUtil.Load<Sprite>(imgPath);
            Get<TextMeshProUGUI>((int)Texts.ItemName).text = itemName;
            Get<TextMeshProUGUI>((int)Texts.ItemInfo).text = itemInfo;
        }
    }
}