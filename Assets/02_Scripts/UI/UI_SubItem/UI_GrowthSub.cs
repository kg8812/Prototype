using System.Collections.Generic;
using Apis.DataType;
using Apis.UI;
using Default;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_GrowthSub : UI_Base
{
    enum Texts
    {
        StatName,
        Description,
        CurLvlValue,
        NextLvlValue,
        CurValue,
        NextValue,
        CostText,
        CostValue,
    }

    enum Buttons
    {
        ApplyButton
    }

    private TextMeshProUGUI statName;
    private TextMeshProUGUI description;
    private TextMeshProUGUI curLvlValue;
    private TextMeshProUGUI nextLvlValue;
    private TextMeshProUGUI curValue;
    private TextMeshProUGUI nextValue;
    private TextMeshProUGUI costText;
    private TextMeshProUGUI costValue;

    public Button applyButton;
    private UIEffector applyBtn;

    private int curLvl;

    public override void Init()
    {
        base.Init();

        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Button>(typeof(Buttons));

        applyBtn = applyButton.GetComponent<UIEffector>();

        statName = GetText((int)Texts.StatName);
        description = GetText((int)Texts.Description);
        curLvlValue = GetText((int)Texts.CurLvlValue);
        nextLvlValue = GetText((int)Texts.NextLvlValue);
        curValue = GetText((int)Texts.CurValue);
        nextValue = GetText((int)Texts.NextValue);
        costValue = GetText((int)Texts.CostValue);
        costText = GetText((int)Texts.CostText);

        applyButton = GetButton((int)Buttons.ApplyButton);
    }

    public void Set(ActorStatType statType, UnityAction<GrowthDataType, UnityAction> action)
    {
        statName.text = Utils.GetStatText(statType);
        if (LobbyDatabase.TryGetGrowthData(statType, out var list))
        {
            UpdateUI(statType, list);
            applyButton.onClick.RemoveAllListeners();
            applyButton.onClick.AddListener(() =>
            {
                action(list[curLvl + 1], () =>
                {
                    UpdateUI(statType, list);
                });
            });
        }
    }

    private void UpdateUI(ActorStatType statType, Dictionary<int, GrowthDataType> list)
    {
        description.text = $"최대 성장 레벨 {list.Keys.Count - 1}";
        curLvl = GameManager.Save.currentSlotData.GrowthSaveData.Growth.CurLvl[statType];
        curLvlValue.text = curLvl.ToString();
        nextLvlValue.text = (int.Parse(curLvlValue.text) + 1).ToString();
        curValue.text = $"+{list[curLvl].value}%";
        nextValue.text = $"+{list[curLvl + 1].value}%";
        int cost = list[curLvl + 1].cost;
        int soul = GameManager.instance.LobbySoul;
        costValue.text = $"{soul}/{cost}";
        
        costText.color = cost > soul ? Color.red : Color.white;
        costValue.color = cost > soul ? Color.red : Color.white;
        if (cost > soul)
        {
            applyBtn.DisableOn();
        }
        else
        {
            applyBtn.DisableOff();
        }
        
    }
}