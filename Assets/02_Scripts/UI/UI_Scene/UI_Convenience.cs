using System.Collections.Generic;
using Apis;
using Apis.DataType;
using Default;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_Convenience : UI_Scene
    {
        public enum GameObjects
        {
            UpgradeParent,
        }

        public enum Texts
        {
            DescriptionText,SoulText,
        }

        public enum Buttons
        {
            CloseButton
        }

        private Transform upgradeParent;

        private Dictionary<int, UI_ConvenienceSub> upgradeDict;
        private TextMeshProUGUI description;

        private Button closeButton;

        [HideInInspector]public UI_ConveniencePopup popup;
        [HideInInspector] public TextMeshProUGUI soulText;
        

        public override void Init()
        {
            base.Init();
            Bind<Transform>(typeof(GameObjects));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Button>(typeof(Buttons));
            upgradeParent = Get<Transform>((int)GameObjects.UpgradeParent);
            upgradeDict = new();
            description = Get<TextMeshProUGUI>((int)Texts.DescriptionText);
            closeButton = Get<Button>((int)Buttons.CloseButton);
            closeButton.onClick.AddListener(() => GameManager.UI.CloseUI(this));
            soulText = Get<TextMeshProUGUI>((int)Texts.SoulText);
            GameManager.instance.OnLobbySoulChange.RemoveListener(UpdateSoulText);
            GameManager.instance.OnLobbySoulChange.AddListener(UpdateSoulText);
        }

        void UpdateSoulText(int soul)
        {
            soulText.text = GameManager.instance.LobbySoul.ToString();
        }
        protected override void Activated()
        {
            base.Activated();
            description.text = "";
            CreateList();
            SetList();
            soulText.text = GameManager.instance.LobbySoul.ToString();
        }

        public void CreateList()
        {
            foreach (var data in LobbyDatabase.convenienceDict.Values)
            {
                if (data.unlock == 0 || DataAccess.LobbyData.IsOpen(data.unlock))
                {
                    if (upgradeDict.ContainsKey(data.index)) continue;

                    UI_ConvenienceSub obj = MakeSub(data);
                    if (!ReferenceEquals(obj, null))
                    {
                        upgradeDict.TryAdd(data.index, obj);
                    }
                }
                else if (upgradeDict.ContainsKey(data.index))
                {
                    GameManager.UI.ReturnUI(upgradeDict[data.index].gameObject);
                    upgradeDict.Remove(data.index);
                }
            }
        }

        public void SetList()
        {
            foreach (var x in upgradeDict.Values)
            {
                x.button.onClick.RemoveAllListeners();
                if (DataAccess.LobbyData.IsOpen(x.Data.index))
                {
                    x.transform.SetAsLastSibling();
                    x.costText.color = Color.grey;
                    x.nameText.color = Color.grey;
                }
                else if (x.Data.priceWarmth > GameManager.instance.LobbySoul)
                {
                    x.costText.color = Color.red;
                    x.nameText.color = Color.red;
                    x.button.onClick.AddListener(() =>
                    {
                        Select(x);
                    });
                }
                else
                {
                    x.costText.color = Color.white;
                    x.nameText.color = Color.white;
                    x.button.onClick.AddListener(() =>
                    {
                        Select(x);
                    });
                }
            }
        }

        UI_ConvenienceSub MakeSub(ConvenienceDataType data)
        {
            if (data != null)
            {
                UI_ConvenienceSub sub = GameManager.UI.MakeUI<UI_ConvenienceSub>("UI_ConvenienceSub", transform);
                sub.Init(data);
                sub.transform.SetParent(upgradeParent);
                AddUIEvent(sub.gameObject, x =>
                {
                    description.text = data.desc;
                },Define.UIEvent.PointEnter);
                
                return sub;
            }

            return null;
        }

        void Select(UI_ConvenienceSub sub)
        {
            popup = GameManager.UI.CreateUI("UI_ConveniencePopup", UIType.Popup) as UI_ConveniencePopup;
            if (ReferenceEquals(popup, null)) return;

            popup.Init(this);
            popup.Set(sub);
        }
        
        

        protected override void Deactivated()
        {
            base.Deactivated();
            if (popup != null)
            {
                GameManager.UI.CloseUI(popup);
            }
        }
    }
}