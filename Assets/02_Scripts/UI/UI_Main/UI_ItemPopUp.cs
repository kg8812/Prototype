using Apis;
using Sirenix.OdinInspector;
using TMPro;
using UISpaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_ItemPopUp : UI_Main
    {
        enum Images
        {
            Icon
        }
        enum Texts
        {
            ItemName
        }

        [LabelText("지속시간")] public float duration;
        TextMeshProUGUI itemName;
        Image icon;
        public override void Init()
        {
            base.Init();
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(Texts));
            icon = Get<Image>((int)Images.Icon);
            itemName = Get<TextMeshProUGUI>((int)Texts.ItemName);
        }
        public void Init(Item item)
        {
            CancelInvoke(nameof(CloseOwn));
            Invoke(nameof(CloseOwn), duration);
            icon.sprite = item.Image;
            itemName.text = StrUtil.GetEquipmentName(item.ItemId);
        }

    }
}