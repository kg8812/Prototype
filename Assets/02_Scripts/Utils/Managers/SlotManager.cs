using Apis;
using Apis;
using Apis.Managers;
using Directing;
using Save.Schema;
using Spine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class SlotManager
    {
        public bool IsProgressing { get; private set; }
        public void CreateNewSlot(string slotId)
        {
            string newId = slotId;
            Save.Schema.SlotData newSlotData = new (newId);
            GameManager.Save.SetCurrentSlotData(newSlotData);
            GameManager.instance.playTime = 0;
            
            return;
            
            void LoadTutorial(Player _)
            {
                GameManager.Scene.SceneLoad(Define.SceneNames.MainWorldSceneName);
                GameManager.Save.ResetSlotData();
                DataAccess.GameData.Save();
            }
        }

        public void LoadSlot(string slotId)
        {
            Debug.Log("Load : " + slotId);
            GameManager.Save.LoadSlotData(slotId);
                
            GameManager.Scene.SceneLoad(Define.SceneNames.MainWorldSceneName);
        }


        public void SaveCurrentSlot()
        {
            if (GameManager.Scene.CurSceneData.isPlayerMustExist && GameManager.instance.Player != null && GameManager.Save.currentSlotData.slotId != "0")
            {
                // GameData에 SlotData 적용시키기전에 Save 먼저해서 SlotData에 플레이어 정보저장 먼저해야됨.
                GameManager.Save.SaveData(SaveManager.SaveType.Slot);

                var curSlotInfoSave = GameManager.Save.currentSlotData;
                
                curSlotInfoSave.UpdateSlotDataToGameData();
            }
        }
        
        public void DeleteSlot(string slotId)
        {
            SlotInfoSaveData data = null;
            
            foreach (var slotInfoSaveData in DataAccess.GameData.Data.SlotDatas)
            {
                if (slotInfoSaveData.SlotId == slotId)
                {
                    data = slotInfoSaveData;
                    break;
                }
            }
        
            if (data != null)
            {
                DataAccess.GameData.Data.SlotDatas.Remove(data);
                ES3.DeleteKey(SaveManager.GetSlotDataKey(slotId));
                DataAccess.GameData.Save();
            }
        }
    }
}