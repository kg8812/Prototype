using System.Collections.Generic;
using Apis;
using Apis.Managers;
using Apis.UI;
using Default;
using Managers;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI.UI_SubItem
{
    public class UI_SaveSlot_SlotItem: UIAsset_Button
    {
        enum GameObjects
        {
            SlotData
        }

        enum Buttons
        {
            DeleteButton
        }
        enum Texts
        {
            // PlayerType, 
            Lv, LastPlayTime, PlaceName, PlayTime, Progress
        }

        enum Images
        {
            Background
        }

        private string mySlotId;
        private UI_SaveSlot _uiSaveSlot;
        private RectTransform rect;
        
        public Sprite[] bgImgs;
        
        public override void Init()
        {
            base.Init();
            Bind<GameObject>(typeof(GameObjects));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Image>(typeof(Images));
            Bind<Button>(typeof(Buttons));
            
            OnClick.AddListener((() =>
            {
                if (_uiSaveSlot.choosed) return;
                int confirmMsgId = string.IsNullOrEmpty(mySlotId) ? 101053 : 101052;
                SystemManager.SystemCheck(LanguageManager.Str(confirmMsgId), todo =>
                {
                    if (todo)
                    {
                        _uiSaveSlot.ChooseSlot(mySlotId);
                    }
                });
            }));
            
            GetButton((int)Buttons.DeleteButton).onClick.AddListener(() =>
            {
                RemoveSlotData();
                _uiSaveSlot.SetSlotList();
            });
        }

        public void SetNew(string slotId, Vector3 pos, UI_SaveSlot parent)
        {
            mySlotId = slotId;
            SetTransform(pos,parent);

            Get<Image>((int)Images.Background).sprite = bgImgs[6];
            // Get<GameObject>((int)GameObjects.NewSlot).SetActive(true);
            Get<GameObject>((int)GameObjects.SlotData).SetActive(false);
        }
        public void SetInfo(string slotId, SlotInfoSaveData data, Vector3 pos, UI_SaveSlot parent)
        {
            mySlotId = slotId;
            SetTransform(pos,parent);

            Get<Image>((int)Images.Background).sprite = bgImgs[(int)data.PlayerType];
            

            Get<TextMeshProUGUI>((int)Texts.Lv).text = data.Lv.ToString();
            Get<TextMeshProUGUI>((int)Texts.LastPlayTime).text = data.LastPlayTime.ToString("MM / dd / yyyy    HH:mm");
            
            Get<TextMeshProUGUI>((int)Texts.PlaceName).text = LanguageManager.Str(data.PlaceNameId);
            Get<TextMeshProUGUI>((int)Texts.PlayTime).text = FormatUtils.TimeDisplay(data.PlayTime);
            Get<TextMeshProUGUI>((int)Texts.Progress).text = Mathf.RoundToInt(data.Progress).ToString();
            
            
            Get<GameObject>((int)GameObjects.SlotData).SetActive(true);
        }

        void SetTransform(Vector3 pos, UI_SaveSlot parent)
        {
            _uiSaveSlot = parent;
            rect = GetComponent<RectTransform>();
            //UI가 Anchor 설정이 되어 있는데 anchoredPosition이 아닌 localPosition으로 위치 설정해서 제대로 적용 안되고 있었습니다.
            // AnchoredPosition으로 변경함
            rect.localScale = Vector3.one;
            rect.anchoredPosition = pos;
        }
        private string PlaceImgPath(int placeId)
        {
            string path = "Sprite/PlaceBackgroundImg/";
            int placeType = (placeId - 50000) / 1000;
            switch (placeType)
            {
                case 1:
                    return path + "Gosegu";
                case 2:
                    return path + "Jururu";
                case 3:
                    return path + "Jingburger";
                case 4:
                    return path + "Lilpa";
                case 5:
                    return path + "Ine";
                case 6:
                    return path + "Viichan";
                // case 0: - 튜토나 로비도 똑같이 default image로
                default:
                    return path + "Default";
            }
        }

        public void RemoveSlotData()
        {
            string temp = mySlotId;
            mySlotId = null;
            GameManager.Slot.DeleteSlot(temp);
        }
    }
}