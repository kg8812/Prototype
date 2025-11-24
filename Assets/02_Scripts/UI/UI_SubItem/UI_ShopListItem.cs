using System;
using Apis;
using Apis.DataType;
using Apis.Managers;
using Apis.UI;
using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI.UI_SubItem
{
    public class UI_ShopListItem: UIAsset_Toggle
    {
        private readonly Color _canNotBuyTextColor = new Color(1, 0, 0);
        
        #region Bindings

        private enum Texts
        {
            Name, Count, Price
        }

        private enum Images
        {
            PriceIcon
        }

        #endregion
        

        public ShopListDataType ItemData { get; private set; }
        public Action<UI_ShopListItem> UpdateShopItem;

        private int _priceTextValue;
        private bool _isBuy;

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Image>(typeof(Images));
            OnValueChanged.AddListener(isOn =>
            {
                if(isOn)
                    UpdateShopItem?.Invoke(this);
            });

            OnDeactivated.AddListener(() =>
            {
                if (!GameManager.IsQuitting)
                {
                    GameManager.instance.OnSoulChange.RemoveListener(UpdateColorByUpdate);
                    GameManager.instance.OnLobbySoulChange.RemoveListener(UpdateColorByUpdate);
                }
            });
        }

        public void UpdateData(ShopListDataType itemData)
        {
            ItemData = itemData;
            GetText((int)Texts.Name).text = StrUtil.GetEquipmentName(ItemData.itemId) ?? "Unknown";
            // TODO: icon 세팅
            GetImage((int)Images.PriceIcon).sprite = GetPriceIcon(ItemData.priceType);
            
            GetText((int)Texts.Count).text = ItemData.count.ToString();
            _priceTextValue = ItemData.price;

            GameManager.instance.OnSoulChange.RemoveListener(UpdateColorByUpdate);
            GameManager.instance.OnLobbySoulChange.RemoveListener(UpdateColorByUpdate);
            if (ItemData.priceType == 0)
            {
                GameManager.instance.OnSoulChange.AddListener(UpdateColorByUpdate);
            }
            else
            {
                GameManager.instance.OnLobbySoulChange.AddListener(UpdateColorByUpdate);
            }

            BuyTextToggle(false);
        }

        private Sprite GetPriceIcon(int priceType)
        {
            switch (priceType)
            {
                // 0 = gold, 1 = lobby gold
                case 0:
                    return ResourceUtil.Load<Sprite>("Sprites/UI/Icon/GoldIcon");
                case 1:
                default:
                    return ResourceUtil.Load<Sprite>("Sprites/UI/Icon/GoldIcon");
            }
        }

        public void BuyTextToggle(bool isBuy)
        {
            // TODO: 나중에 Ui 바뀌면 업데이트.
            _isBuy = isBuy;
            GetText((int)Texts.Price).text = _isBuy ? "이미 삼" : _priceTextValue.ToString();
            UpdateColor();
        }

        public void UpdateColor()
        {
            if (ItemData == null) return;
            GetText((int)Texts.Price).color =
                (ItemData.priceType == 0 ? GameManager.instance.Soul : GameManager.instance.LobbySoul) < ItemData.price && !_isBuy ? _canNotBuyTextColor : Color.white;
        }


        private void UpdateColorByUpdate(int _)
        {
            UpdateColor();
        }
    }
}