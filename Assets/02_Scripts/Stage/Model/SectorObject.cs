using System;
using Apis;
using Save.Schema;
using Sirenix.OdinInspector;
using UnityEngine;

public class SectorObject : SerializedMonoBehaviour
{
    [ReadOnly] public Guid guid;
    private Sector _sector;
    private Sector Sector => _sector ??= transform.root.GetComponentInChildren<Sector>(true);

    public Action OnActive;
    private void Awake()
    {
        if (Sector != null && Sector.sectorObjects.ContainsKey(this))
        {
            guid = Sector.sectorObjects[this];
        }
        else
        {
            guid = Guid.Empty;
        }
        
        // 고정 sector object인데, 미리 먹은 경우
        if (GameManager.Save.currentSlotData.SectorSaveData.objectObtained.Contains(guid.ToString()))
        {
            gameObject.SetActive(false);
        }
        // else
        // {
        //     if(GameManager.Save.currentSlotData.SectorSaveData.saveSectorObjects.TryGetValue(guid.ToString(), out var value))
        //     {
        //         // 발견된 고정 sector object이거나, 새롭게 생성된 sector object인 경우 => 저장된 pos 데이터 로드.
        //         // TODO: sector object가 sector가 옮겨지는 경우라면 awake되기 전에 그 자리에 있어야 함으로 
        //         transform.position = value.position;
        //     }
        //     else
        //     {
        //         GameManager.Save.currentSlotData.SectorSaveData.saveSectorObjects.Add(guid.ToString(), new SectorSaveData.SaveSectorObject()
        //         {
        //             prefabAddress = prefabAddress,
        //             position = transform.position
        //         });
        //     }
        // }
    }

    public void Activate()
    {
        if (guid != Guid.Empty)
        {
            Map.instance.ObtainedObject(guid.ToString());
            GameManager.Save.currentSlotData.SectorSaveData.objectObtained.Add(guid.ToString());
            OnActive?.Invoke();
            // GameManager.Save.currentSlotData.SectorSaveData.saveSectorObjects.Remove(guid.ToString());
        }
    }
}
