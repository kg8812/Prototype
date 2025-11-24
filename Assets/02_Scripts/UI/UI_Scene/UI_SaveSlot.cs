using System;
using System.Collections.Generic;
using Default;
using Save.Schema;
using UI;
using UI.UI_SubItem;
using UnityEngine;

namespace Apis
{
    public class UI_SaveSlot : UI_Scene
    {
        private const int MaxSaveSlot = 6;
        
        enum GameObjects
        {
            SlotContent
        }


        public bool choosed { get; private set; }
        private GameObject contentPanel;

        private float minSpace = 50;
        private float slotHeight = 250;
        
        public override void Init()
        {
            base.Init();

            Bind<GameObject>(typeof(GameObjects));
            contentPanel = Get<GameObject>((int)GameObjects.SlotContent);
        }

        public override void TryActivated(bool force = false)
        {
            choosed = false;
            SetSlotList();
            base.TryActivated(force);
        }

        public void SetSlotList()
        {
            foreach (Transform child in contentPanel.transform)
                Destroy(child.gameObject);
            
            var slotDatas = DataAccess.GameData.Data.SlotDatas;
            RectTransform rt = contentPanel.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector3(rt.sizeDelta.x, (minSpace * slotDatas.Count) + (slotHeight* (slotDatas.Count+1)));
            
            for (int i = 0; i <= slotDatas.Count; i++)
            {
                UI_SaveSlot_SlotItem saveSlotItem =
                    GameManager.UI.MakeSubItem("UI_SaveSlot_SlotItem", contentPanel.transform) as UI_SaveSlot_SlotItem;
                Vector3 pos = new Vector3(0, -(minSpace + slotHeight) * i, 0);
                
                if (i == slotDatas.Count)
                {
                    if(i < MaxSaveSlot)
                        saveSlotItem.SetNew(null,pos,this);
                }
                else
                {
                    saveSlotItem.SetInfo(
                        slotDatas[i].SlotId,
                        slotDatas[i],
                        pos, this);
                }
            }
        }

        public void ChooseSlot(string slotId)
        {
            if (choosed) return;
            choosed = true;
            FadeManager.instance.fadeIn.AddListener(CloseOwn);
            
            if(string.IsNullOrEmpty(slotId))
            {
                GameManager.Slot.CreateNewSlot(Guid.NewGuid().ToString());
            }
            else
            {
                GameManager.Slot.LoadSlot(slotId);
            }
        }
    }
}