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


namespace Apis.UI.UI_SubItem
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
            Lv, LastPlayTime, PlayTime
        }

        private string mySlotId;
        private UI_SaveSlot _uiSaveSlot;
        private RectTransform rect;
        
        public override void Init()
        {
            base.Init();
            Bind<GameObject>(typeof(GameObjects));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<Button>(typeof(Buttons));
            
            OnClick.AddListener((() =>
            {
                if (_uiSaveSlot.choosed) return;
                string confirmMsg = string.IsNullOrEmpty(mySlotId) ? "새로운 슬롯을 생성하시겠습니까?" : "해당 슬롯을 플레이하시겠습니까?";
                SystemManager.SystemCheck(confirmMsg, todo =>
                {
                    if (todo)
                    {
                        _uiSaveSlot.ChooseSlot(mySlotId);
                    }
                });
            }));
            
            GetButton((int)Buttons.DeleteButton).onClick.AddListener(() =>
            {
                SystemManager.SystemCheck("데이터를 삭제하시겠습니까?", x =>
                {
                    if (x)
                    {
                        RemoveSlotData();
                        _uiSaveSlot.SetSlotList();
                    }
                });
                
            });
        }

        public void SetNew(string slotId, Vector3 pos, UI_SaveSlot parent)
        {
            mySlotId = slotId;
            SetTransform(pos,parent);

            // Get<GameObject>((int)GameObjects.NewSlot).SetActive(true);
            Get<GameObject>((int)GameObjects.SlotData).SetActive(false);
        }
        public void SetInfo(string slotId, SlotInfoSaveData data, Vector3 pos, UI_SaveSlot parent)
        {
            mySlotId = slotId;
            SetTransform(pos,parent);


            Get<TextMeshProUGUI>((int)Texts.Lv).text = data.Lv.ToString();
            Get<TextMeshProUGUI>((int)Texts.LastPlayTime).text = data.LastPlayTime.ToString("MM / dd / yyyy    HH:mm");
            
            Get<TextMeshProUGUI>((int)Texts.PlayTime).text = FormatUtils.TimeDisplay(data.PlayTime);
            
            
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