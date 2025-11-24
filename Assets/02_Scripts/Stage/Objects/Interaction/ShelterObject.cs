using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Default;
using Directing;
using Managers;
using Save.Schema;
using Sirenix.OdinInspector;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShelterObject : MonoBehaviour , IOnInteract
{
    public Func<bool> InteractCheckEvent { get; set; }

    private int sectorId;
    public int shelterId;

    [SerializeField] private float appearTime;
    [SerializeField] GameObject onResources;
    [SerializeField] List<Renderer> _renderers;
    [SerializeField] private int prerequisiteTipVideoId;

    private bool isActivated;
    
    MaterialPropertyBlock propBlock;
    private void Awake()
    {
        isActivated = false;
        propBlock = new MaterialPropertyBlock();
        if (transform.parent.TryGetComponent(out Sector sector))
        {
            sectorId = sector.sectorID;
        }
        else
        {
            sectorId = -1;
        }

        if (Map.instance.OpenedShelters.ContainsKey(shelterId))
        {
            Activate();
        }
        else
        {
            onResources.SetActive(false);
        }
    }

    public void OnInteract()
    {
        // check
        if (prerequisiteTipVideoId != 0 && !DataAccess.Codex.IsOpen(CodexData.CodexType.Tip, prerequisiteTipVideoId))
        {
            UI_Tip tipUi = GameManager.UI.CreateUI("UI_Tip", UIType.Scene, withoutActivation:true) as UI_Tip;
            if (tipUi != null && TipDatabase.TryGetTipData(prerequisiteTipVideoId, out var data))
            {
                tipUi.InitData(data);
                tipUi.TryActivated();
            }
            DataAccess.Codex.UnLock(CodexData.CodexType.Tip, prerequisiteTipVideoId);
            return;
        }
        
        GameManager.instance._onRest?.Invoke();
        Player player = GameManager.instance.Player;
        player.ResetPlayerStatus();
        player.ControlOff();
        GameManager.SectorMag.SetLastShelter();
        FadeManager.instance.Fading(() =>
        {
            TargetGroupCamera.instance.AdjustTargetRadius(CameraManager.instance.fakePlayerTarget.transform,GameManager.instance.Player.camRadius / 2);
            GameManager.UI.CreateUI("UI_Shelter", UIType.Scene);
        },enterDefaultState:false,endAction: Activate);

        if (sectorId != -1)
        { 
            GameManager.SectorMag.CurrentSectors[SectorManager.SectorType.MainWorld] = sectorId;
        }

        GameManager.SectorMag.ResetSectors();
        GameManager.instance.SaveSlot();
    }

    public void Activate()
    {
        if (isActivated) return;
        isActivated = true;
        onResources.SetActive(true);
        Map.instance.OpenedShelters.Add(sectorId, new SectorSaveData.ShelterData()
        {
            savedPosition = transform.position,
            sceneName = SceneManager.GetActiveScene().name,
        });
        _renderers.ForEach(x =>
        {
            x.GetPropertyBlock(propBlock);
            x.material.SetFloat("_Condition",1);
            x.material.SetFloat("_ApparePow",0);
            x.SetPropertyBlock(propBlock);
        });
        
        StartCoroutine(Appear());
    }

    IEnumerator Appear()
    {
        float curTime = 0;

        while (curTime < appearTime)
        {
            curTime += Time.deltaTime;
            var curAmount = curTime / appearTime;
            foreach (var x in _renderers)
            {
                x.GetPropertyBlock(propBlock);
                x.material.SetFloat("_ApparePow",curAmount);
                x.SetPropertyBlock(propBlock);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
