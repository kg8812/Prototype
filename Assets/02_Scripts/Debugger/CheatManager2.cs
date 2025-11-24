using System;
using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.SkillTrees;
using Apis;
using Apis.Managers;
using Default;
using NewNewInvenSpace;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CheatManager2 : UI_Scene
{
    enum GameObjects
    {
        BaseLayer,WeaponPages,AccessoryPages
    }
    enum Inputs
    {
        WeaponInput,
        AccInput,
        SoulInput,
        LobbySoulInput,
        LevelInput
    }

    enum Buttons
    {
        Weapon,Accessory,InvenPreventBtn
    }

    enum Texts
    {
        InvenPreventToggleText
    }

    private Button weapon;
    private Button acc;
    
    private TMP_InputField weaponInput;
    private TMP_InputField accInput;
    private TMP_InputField soulInput;
    private TMP_InputField lobbySoulInput;
    private TMP_InputField levelInput;

    private UI_CheatWeaponPage weaponPages;
    private UI_CheatAccessoryPage accPages;

    private GameObject baseLayer;

    public override void Init()
    {
        base.Init();
        Bind<TMP_InputField>(typeof(Inputs));
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<TextMeshProUGUI>(typeof(Texts));

        baseLayer = Get<GameObject>((int)GameObjects.BaseLayer);

        weaponPages = Get<GameObject>((int)GameObjects.WeaponPages).GetComponent<UI_CheatWeaponPage>();
        accPages = Get<GameObject>((int)GameObjects.AccessoryPages).GetComponent<UI_CheatAccessoryPage>();
        weapon = GetButton((int)Buttons.Weapon);
        acc = GetButton((int)Buttons.Accessory);
        weaponInput = Get<TMP_InputField>((int)Inputs.WeaponInput);
        accInput = Get<TMP_InputField>((int)Inputs.AccInput);
        soulInput = Get<TMP_InputField>((int)Inputs.SoulInput);
        lobbySoulInput = Get<TMP_InputField>((int)Inputs.LobbySoulInput);
        levelInput = Get<TMP_InputField>((int)Inputs.LevelInput);

        weaponInput.onSubmit.AddListener(x =>
        {
            if (int.TryParse(x,out int result))
            {
                AddWeapon(result);
            }
        });
        accInput.onSubmit.AddListener(x =>
        {
            if (int.TryParse(x,out int result))
            {
                AddAcc(result);
            }
        });
        soulInput.onSubmit.AddListener(x =>
        {
            if (int.TryParse(x,out int result))
            {
                AddSoul(result);
            }
        });
        lobbySoulInput.onSubmit.AddListener(x =>
        {
            if (int.TryParse(x,out int result))
            {
                AddLobbySoul(result);
            }
        });
        levelInput.onSubmit.AddListener(x =>
        {
            if (int.TryParse(x, out int result))
            {
                GameManager.instance.Level = result;
                GameManager.instance.Exp = 0;
                levelInput.text = "";
            }
        });
        weapon.onClick.RemoveAllListeners();
        weapon.onClick.AddListener(OpenWeaponPage);
        acc.onClick.RemoveAllListeners();
        acc.onClick.AddListener(OpenAccPage);
        weaponPages.backButton.onClick.RemoveAllListeners();
        weaponPages.backButton.onClick.AddListener(CloseWeaponPage);
        accPages.backButton.onClick.RemoveAllListeners();
        accPages.backButton.onClick.AddListener(CloseAccPage);

        GetText((int)Texts.InvenPreventToggleText).text = UITab_Inventory.UsePrevent ? "인벤 이동 Off" : "인벤 이동 On";
        GetButton((int)Buttons.InvenPreventBtn)?.onClick.AddListener(() =>
        {
            UITab_Inventory.UsePrevent = !UITab_Inventory.UsePrevent;
            GetText((int)Texts.InvenPreventToggleText).text = UITab_Inventory.UsePrevent ? "인벤 이동 Off" : "인벤 이동 On";
        });
    }

    protected override void Activated()
    {
        base.Activated();
        soulInput.text = "";
        lobbySoulInput.text = "";
        baseLayer.SetActive(true);        
        weaponPages.gameObject.SetActive(false);
        accPages.gameObject.SetActive(false);

    }

    protected override void Deactivated()
    {
        base.Deactivated();
        weaponPages.gameObject.SetActive(false);
        accPages.gameObject.SetActive(false);
    }

    public void AddSoul(int amount)
    {
        soulInput.text = "";
        GameManager.instance.Soul += amount;
    }

    public void AddLobbySoul(int amount)
    {
        lobbySoulInput.text = "";
        GameManager.instance.LobbySoul += amount;
    }

    void OpenWeaponPage()
    {
        baseLayer.SetActive(false);
       weaponPages.gameObject.SetActive(true);
    }

    void CloseWeaponPage()
    {
        baseLayer.SetActive(true);
        weaponPages.gameObject.SetActive(false);
    }
    void OpenAccPage()
    {
        baseLayer.SetActive(false);

        accPages.gameObject.SetActive(true);
    }

    void CloseAccPage()
    {
        baseLayer.SetActive(true);
        accPages.gameObject.SetActive(false);

    }
    public void AddAcc(int index)
    {
        accInput.text = "";
        if (!AccessoryData.DataLoad.TryGetData(index, out var data)) return;
        // string n = LanguageManager.Str(data.accName);
        if (InvenManager.instance.Acc.IsFull(InvenType.Storage))
        {
            var pickUp = GameManager.Item.AccPickUp.CreateNew(data.accId);
            pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
        }
        else
        {
            var acc = GameManager.Item.GetAcc(data.accId);
            InvenManager.instance.Acc.Add(acc, InvenType.Storage);
        }
    }

    public void AddWeapon(int index)
    {
        weaponInput.text = "";
        if (!WeaponData.DataLoad.TryGetWeaponData(index, out var data)) return;
        // string n = LanguageManager.Str(data.weaponNameString);
        if (InvenManager.instance.AttackItem.IsFull(InvenType.Storage))
        {
            var pickUp = GameManager.Item.WeaponPickUp.CreateNew(data.weaponId);
            pickUp.transform.position = GameManager.instance.ControllingEntity.transform.position;
        }
        else
        {
            var wp = GameManager.Item.GetWeapon(data.weaponId);
            InvenManager.instance.AttackItem.Add(wp, InvenType.Storage);
        }
    }

    void AddSkillTree(int index)
    {
        //SkillTreeDatas.Activate(index);
    }

    void RemoveSkillTree(int index)
    {
        SkillTreeDatas.DeApplySkillTree(index);
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetPlayer()
    {
        var p  = GameManager.instance.Player;
        if(p == null) return;

        p.ForcedResetState();
    }

    public void ActivateSkills()
    {
        DataAccess.TaskData.ActivateTask(102);
        DataAccess.TaskData.ActivateTask(103);
        GameManager.instance.Player.ResetActiveSkill();
        GameManager.instance.Player.ResetPassiveSkill();

    }
}
