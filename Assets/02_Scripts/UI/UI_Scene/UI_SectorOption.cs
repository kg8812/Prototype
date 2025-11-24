using UnityEngine;
using Apis.DataType;
using Default;

namespace UI
{
    public class UI_SectorOption : UI_Scene
    {
        enum GameObjects
        {
            Content
        }

        enum Texts
        {
            Title
        }
        
        
        private const float minSpace = 470;

        private bool isChoosed = false;
        private GameObject gridPanel;

        public override void Init()
        {
            base.Init();

            Bind<GameObject>(typeof(GameObjects));
            // Bind<TextMeshProUGUI>(typeof(Texts));

            gridPanel  = Get<GameObject>((int)GameObjects.Content);
            
        }

        public override void TryActivated(bool force = false)
        {
            isChoosed = false;
            foreach (Transform child in gridPanel.transform)
                Destroy(child.gameObject);

            // if (GameManager.Sector.IsLastFloor())
            // {
            //     // TODO: Change To StingTable
            //     Get<TextMeshProUGUI>((int)Texts.Title).text = "다음 스테이지로";
            // }
            // else
            // {
            //     // TODO: Change To StingTable
            //     Get<TextMeshProUGUI>((int)Texts.Title).text = "다음 층으로";
            // }

            // SectorDataType[] sectorDatas = GameManager.SectorMag.GetRandomSectorOptions();
            
            // for (int i = 0; i < sectorDatas.Length; i++)
            // {
            //     UI_SectorOption_Item sectorOptionItem = GameManager.UI.MakeSubItem("UI_SectorOption_Item", gridPanel.transform) as UI_SectorOption_Item;
            //     Vector3 pos = new Vector3(minSpace * (i - ((float)(sectorDatas.Length - 1) / 2)), 0, 0);
            //     sectorOptionItem.SetInfo(sectorDatas[i], pos, this);
            // }
            base.TryActivated(force);
        }
        

        public void ChooseSector(int sectorId)
        {
            if (!isChoosed)
            {
                isChoosed = true;
                FadeManager.instance.fadeIn.AddListener(() =>
                {
                    GameManager.UI.CloseUI(this);
                });
                // GameManager.SectorMag.ChooseSectorOption(sectorId);
            }
            else
            {
                Debug.LogWarning("Sector is already choose");
            }
        }
    }
}