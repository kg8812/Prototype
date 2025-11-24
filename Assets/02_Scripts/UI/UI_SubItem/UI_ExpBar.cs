using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.DataType;
using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ExpBar : UI_Base
{
    public Image expBar;
    public TextMeshProUGUI levelText;

    public override void Init()
    {
        base.Init();
        GameManager.instance.levelChange += SetLevelText;
        GameManager.instance.expChange += SetExpBar;
        SetExpBar(0);
        SetLevelText(1);
    }

    void SetExpBar(int exp)
    {
        int maxExp = LevelDatabase.GetLevelData(GameManager.instance.Level).exp;
        expBar.fillAmount = exp / (float)maxExp;
    }

    void SetLevelText(int level)
    {
        levelText.text = level.ToString();
    }
}
