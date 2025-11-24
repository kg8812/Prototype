using Apis.DataType;
using Apis.Managers;
using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_SectorOption_Item: UI_Base
    {
        enum Buttons
        {
            SelectBtn
        }

        enum Texts
        {
            SectorName,
            SectorDescription
        }

        enum Images
        {
            IconImg
        }

        private UI_SectorOption _uiSectorOption;
        private SectorDataType _sectorDataType;

        private RectTransform rect;
        
        public override void Init()
        {
            base.Init();
            Bind<Button>(typeof(Buttons));
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(Texts));
            
            Button myBtn = Get<Button>((int)Buttons.SelectBtn);
            myBtn.onClick.AddListener(SelectBtn);
            
        }

        public void SetInfo(SectorDataType sectorDataType , Vector3 pos, UI_SectorOption uiSectorOption)
        {
            _uiSectorOption = uiSectorOption;
            _sectorDataType = sectorDataType;
            rect = GetComponent<RectTransform>();
            rect.localPosition = pos;
            rect.localScale = Vector3.one;
            Get<TextMeshProUGUI>((int)Texts.SectorName).text = LanguageManager.Str(int.Parse($"5{_sectorDataType.sectorId}"));
            Get<TextMeshProUGUI>((int)Texts.SectorDescription).text = LanguageManager.Str(int.Parse($"8{_sectorDataType.sectorId}"));
            /* TODO: ui icon 넘어오면 적용
            Sprite loadedSprite = ResourceUtil.Load<Sprite>(GetIconPath(_sectorDataType.iconImg));
            if (loadedSprite != null)
            {
                Get<Image>((int)Images.IconImg).sprite = loadedSprite;
            }
            */
        }

        private void SelectBtn()
        {
            _uiSectorOption.ChooseSector(_sectorDataType.sectorId);
            // SystemManager.SystemCheck("정말로 해당 스테이지로 들어가시겠습니까?", (bool isYes) =>
            // {
            //     if (isYes)
            //     {
            //         
            //     }
            // });
            
        }
        
        
        // TODO: Icon Sprite Path 입력
        private string GetIconPath(string iconName) => $"/{iconName}.png";
    }
}