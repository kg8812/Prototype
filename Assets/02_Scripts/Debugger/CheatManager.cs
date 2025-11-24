using Apis;
using Apis;
using Apis.Managers;
using Default;
using NewNewInvenSpace;
using TMPro;

public class CheatManager : UI_Scene
{
    enum Inputs
    {
        WeaponInput,
        AccInput,
        SoulInput,
        LobbySoulInput,
    }

    private TMP_InputField weaponInput;
    private TMP_InputField accInput;
    private TMP_InputField soulInput;
    private TMP_InputField lobbySoulInput;
    
    public override void Init()
    {
        base.Init();
        Bind<TMP_InputField>(typeof(Inputs));

        weaponInput = Get<TMP_InputField>((int)Inputs.WeaponInput);
        accInput = Get<TMP_InputField>((int)Inputs.AccInput);
        soulInput = Get<TMP_InputField>((int)Inputs.SoulInput);
        lobbySoulInput = Get<TMP_InputField>((int)Inputs.LobbySoulInput);
        
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
    }


    public override void TryActivated(bool force = false)
    {
        weaponInput.text = "";
        accInput.text = "";
        soulInput.text = "";
        lobbySoulInput.text = "";
        base.TryActivated(force);
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
            // InvenManager.instance.AttackItem.Add(wp, InvenType.Inven);
        }
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
}
