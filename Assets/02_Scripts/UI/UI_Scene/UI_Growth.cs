using Default;
using Apis.DataType;
using Managers;
using Save.Schema;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Growth : UI_Scene
{
    public enum Buttons
    {
        CloseButton,YesButton,NoButton,
    }

    enum Subs
    {
        Atk,Hp,Def,MoveSpeed,Crit
    }

    enum Texts
    {
        PopupText
    }
    
    private UI_GrowthSub atk;
    private UI_GrowthSub hp;
    private UI_GrowthSub def;
    private UI_GrowthSub moveSpeed;
    private UI_GrowthSub crit;

    // private Button yes;
    // private Button no;

    // public GameObject popup;
    //
    // private TextMeshProUGUI popupText;
    
    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<UI_GrowthSub>(typeof(Subs));
        Bind<TextMeshProUGUI>(typeof(Texts));
        
        Get<Button>((int)Buttons.CloseButton).onClick.AddListener(() =>
        {
            GameManager.UI.CloseUI(this);
        });
        
        atk = Get<UI_GrowthSub>((int)Subs.Atk);
        hp = Get<UI_GrowthSub>((int)Subs.Hp);
        def = Get<UI_GrowthSub>((int)Subs.Def);
        moveSpeed = Get<UI_GrowthSub>((int)Subs.MoveSpeed);
        crit = Get<UI_GrowthSub>((int)Subs.Crit);
        
        atk.Init();
        hp.Init();
        def.Init();
        moveSpeed.Init();
        crit.Init();
        
        atk.Set(ActorStatType.Atk, SetPopup);
        hp.Set(ActorStatType.MaxHp, SetPopup);
        def.Set(ActorStatType.Def, SetPopup);
        moveSpeed.Set(ActorStatType.MoveSpeed, SetPopup);
        crit.Set(ActorStatType.CritProb, SetPopup);

        // yes = GetButton((int)Buttons.YesButton);
        // no = GetButton((int)Buttons.NoButton);
        //
        // no.onClick.AddListener(() => popup.SetActive(false));
        //
        // popupText = GetText((int)Texts.PopupText);
    }

    void SetPopup(GrowthDataType data, UnityAction onUpdate)
    {
        if (GameManager.instance.LobbySoul < data.cost) return;
        SystemManager.SystemCheck(Utils.GetStatText(data.statGroup) + "을(를) 성장시키겠습니까?", (isYes) =>
        {
            if (isYes)
            {
                GameManager.instance.LobbySoul -= data.cost;
                GameManager.Save.currentSlotData.GrowthSaveData.Player.baseStat.Add(data.statGroup, data.addValue);
                GameManager.Save.currentSlotData.GrowthSaveData.Growth.CurLvl[data.statGroup] = data.lvl;
                onUpdate.Invoke();
                // popup.SetActive(false);
            }
        });
    }
}
