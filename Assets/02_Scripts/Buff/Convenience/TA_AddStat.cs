using System.Collections;
using System.Collections.Generic;
using Apis;
using Save.Schema;
using UnityEngine;

public class TA_AddStat : ITriggerActivate
{
    public PermanentGrowthSaveData.PlayerData Stat;

    public TA_AddStat(PermanentGrowthSaveData.PlayerData stat)
    {
        Stat = stat;
    }
    public void Activate()
    {
        GameManager.Save.currentSlotData.GrowthSaveData.Player.playerStat += Stat.playerStat;
        GameManager.Save.currentSlotData.GrowthSaveData.Player.baseStat += Stat.baseStat;
        GameManager.instance.Player.UpdatePlayerStat();
    }
}
