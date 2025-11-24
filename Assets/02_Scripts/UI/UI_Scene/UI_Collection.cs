using System.Collections.Generic;
using System.Text;
using Apis;
using Apis;
using Apis.Managers;
using Apis.UI;
using Apis.UI.Focus;
using UnityEngine;
using Default;
using Save.Schema;
using TMPro;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class UI_Collection : UI_HeaderMenu_nto1
{
    enum Texts
    {
        ListTitle,
        DescriptionText,
        NameText,
    }

    enum Images
    {
        Image
    }
    
    enum GameObjects
    {
        ImageObject,
    }
    
    public enum Buttons
    {
        BackButton
    }

    enum TitleList
    {
        Weapon = 0,
        Acc,
        Enemy,
        Story,
        Etc,
    }

    public FocusParent focusParent;

    private TextMeshProUGUI collectionTitle;
    private TextMeshProUGUI descriptionText;
    private TextMeshProUGUI nameText;

    private Image image;
    private GameObject imageObj;

    public ScrollRect scroll;


    private StringBuilder namestr = new StringBuilder();
    private StringBuilder description = new StringBuilder();


    private List<UIAsset_Toggle> list = new();
    public Transform listParent;

    public override void Init()
    {
        base.Init();
        Bind<TextMeshProUGUI>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        collectionTitle = Get<TextMeshProUGUI>((int)Texts.ListTitle);
        descriptionText = Get<TextMeshProUGUI>((int)Texts.DescriptionText);
        nameText = GetText((int)Texts.NameText);
        imageObj = Get<GameObject>((int)GameObjects.ImageObject);
        

        image = GetImage((int)Images.Image);
        Get<Button>((int)Buttons.BackButton).onClick.AddListener(() => GameManager.UI.CloseUI(this));
        
    }

    protected override void FocusChanged(int id)
    {
        SetList((TitleList)id);
        base.FocusChanged(id);
    }

    private void Update()
    {
        
    }

    private string GetCollectionTitle(int titleType)
        => LanguageManager.Str(1011121 + titleType);

    private void SetList(TitleList type)
    {
        HashSet<int> set;
        scroll.verticalNormalizedPosition = 1;
        int i = 0;
        nameText.text = "";
        descriptionText.text = "";
        collectionTitle.text = GetCollectionTitle((int)type);
        switch (type)
        {
            case TitleList.Weapon:
                set = DataAccess.Codex.DataInt(CodexData.CodexType.Item);
                break;
            case TitleList.Acc:
                set = DataAccess.Codex.DataInt(CodexData.CodexType.Item);
                break;
            case TitleList.Enemy:
            case TitleList.Story:
            case TitleList.Etc:
            default:
                return;
        }

        foreach (var x in set)
        {
            namestr.Clear();
            description.Clear();

            switch (type)
            {
                case TitleList.Weapon:
                    if (!WeaponData.DataLoad.TryGetWeaponData(x, out var data1)) continue;
                    namestr.Append(StrUtil.GetEquipmentName(data1.weaponId));
                    description.Append(StrUtil.GetEquipmentDesc(data1.weaponId));
                    break;
                case TitleList.Acc:
                    if (!AccessoryData.DataLoad.TryGetData(x, out var data2)) continue;
                    namestr.Append(StrUtil.GetEquipmentName(data2.accId));
                    description.Append(StrUtil.GetEquipmentDesc(data2.accId));
                    break;
            }

            UIAsset_Toggle uiElement;
            if (i < list.Count)
            {
                list[i].gameObject.SetActive(true);
                uiElement = list[i];
            }
            else
            {
                uiElement = GameManager.UI.MakeSubItem("CollectionButton", transform)
                    .GetComponent<UIAsset_Toggle>();
                uiElement.transform.SetParent(listParent);
                focusParent.RegisterElement(uiElement);
                list.Add(uiElement);
            }

            uiElement.GetComponentInChildren<TextMeshProUGUI>().text = namestr.ToString();
            uiElement.OnValueChanged.RemoveAllListeners();
            uiElement.OnValueChanged.AddListener(isTrue =>
            {
                if (isTrue)
                {
                    nameText.text = namestr.ToString();
                    descriptionText.text = description.ToString();
                    imageObj.SetActive(true);
                    scroll.UpdateSelectedChildToScrollView(uiElement.rectTransform);
                }
            });

            // UI_Selectable s = uiElement.GetComponent<UI_Selectable>();
            // s.OnSelected.RemoveAllListeners();
            // s.OnSelected.AddListener(() =>
            // {
            //     // if (a is AxisEventData)
            //     // {
            //     //     
            //     // }
            //
            //     // Vector2 pos = scroll.content.localPosition;
            //     // pos.y = scroll.GetSnapToPositionToBringChildIntoView(button.myBtn.image.rectTransform).y;
            //     // scroll.content.localPosition = pos;
            //     
            // });
            i++;
        }
    }
}
/*
enum Texts
{
    ListTitle,DescriptionText,NameText,
}

enum Images
{
    Image
}
enum GameObjects
{
    Selected_1 = 0,
    Selected_2,
    Selected_3,
    Selected_4,
    Selected_5,
    ImageObject,
}

public enum Buttons
{
   BackButton,Weapon,Acc,Enemy,Story,Ex
}

enum TitleList
{
    Weapon = 0,Acc,Enemy,Story,Etc,
}
TextMeshProUGUI collectionTitle;
TextMeshProUGUI descriptionText;
private TextMeshProUGUI nameText;
private GameObject imageObj;

private Dictionary<int, GameObject> selected = new();

private Image image;
private Button weapon;
private Button acc;
private Button enemy;
private Button story;
private Button ex;

public ScrollRect scroll;

public override void Init()
{
    base.Init();
    Bind<TextMeshProUGUI>(typeof(Texts));
    Bind<GameObject>(typeof(GameObjects));
    Bind<Button>(typeof(Buttons));
    Bind<Image>(typeof(Images));
    collectionTitle = Get<TextMeshProUGUI>((int)Texts.ListTitle);
    descriptionText = Get<TextMeshProUGUI>((int)Texts.DescriptionText);
    nameText = GetText((int)Texts.NameText);
    weapon = GetButton((int)Buttons.Weapon);
    acc = GetButton((int)Buttons.Acc);
    enemy = GetButton((int)Buttons.Enemy);
    story = GetButton((int)Buttons.Story);
    ex = GetButton((int)Buttons.Ex);
    imageObj = Get<GameObject>((int)GameObjects.ImageObject);
    for (int i = 0; i < 5; i++)
    {
        selected.Add(i, Get<GameObject>(i));
    }

    image = GetImage((int)Images.Image);
    Get<Button>((int)Buttons.BackButton).onClick.AddListener(() => GameManager.UI.CloseSceneUI(this));
}
public void Select(int button)
{
    foreach (var x in selected.Keys)
    {
        selected[x].SetActive(x == button);
    }
    SetList((TitleList)button);
}

private List<Button> list = new();
public Transform listParent;

void SetList(TitleList type)
{
    HashSet<int> set;
    int i = 0;
    scroll.verticalNormalizedPosition = 1;
    nameText.text = "";
    descriptionText.text = "";
    imageObj.SetActive(false);
    switch (type)
    {
        case TitleList.Weapon:
            collectionTitle.text = "무기 리스트";
            set = GameManager.Save.CodexData.DataInt(CodexData.CodexType.Item);
            break;
        case TitleList.Acc:
            collectionTitle.text = "악세서리 리스트";
            set = GameManager.Save.CodexData.DataInt(CodexData.CodexType.Item);
            break;
        case TitleList.Etc:
        case TitleList.Enemy:
        case TitleList.Story:
        default:
            return;
    }

    foreach (var x in set)
    {
        string namestr = "";
        string description = "";
        switch (type)
        {
            case TitleList.Weapon:
                if (!WeaponData.DataLoad.TryGetWeaponData(x, out var data1)) continue;
                namestr = LanguageManager.Str(data1.weaponNameString);
                description = LanguageManager.Str(data1.description);
                break;
            case TitleList.Acc:
                if (!AccessoryData.DataLoad.TryGetData(x, out var data2)) continue;
                namestr = LanguageManager.Str(data2.accName);
                description = LanguageManager.Str(data2.accDesc);
                break;
        }

        Button button;
        if (i < list.Count)
        {
            list[i].gameObject.SetActive(true);
            button = list[i];
        }
        else
        {
            button = GameManager.UI.MakeSubItem("CollectionButton", transform)
                .GetComponent<Button>();
            button.transform.SetParent(listParent);
            list.Add(button);
        }
        button.GetComponentInChildren<TextMeshProUGUI>().text = namestr;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            nameText.text = namestr;
            descriptionText.text = description;
            imageObj.SetActive(true);
        });
        UI_Selectable s = button.GetComponent<UI_Selectable>();
        // s.OnSelectEvent.RemoveAllListeners();
        // s.OnSelectEvent.AddListener(a =>
        // {
        //     if (a is AxisEventData)
        //     {
        //         Vector2 pos = scroll.content.localPosition;
        //         pos.y = scroll.GetSnapToPositionToBringChildIntoView(button.image.rectTransform).y;
        //         scroll.content.localPosition = pos;
        //     }
        // });
        i++;
    }

    for (int j = i; j < list.Count; j++)
    {
        list[i].gameObject.SetActive(false);
    }

    if (list.Count > 0 && list[0].gameObject.activeSelf)
    {
       SetNavigation(weapon,list[0]);
       SetNavigation(acc,list[0]);
       SetNavigation(enemy,list[0]);
       SetNavigation(story,list[0]);
       SetNavigation(ex,list[0]);
    }
}
void SetNavigation(Selectable button,Selectable target)
{
    Navigation navigation = button.navigation;
    navigation.selectOnDown = target;
    button.navigation = navigation;
}
public override void Activated()
{
    base.Activated();
    foreach (var x in selected.Values)
    {
        x.SetActive(false);
    }
    weapon.Select();
    weapon.onClick.Invoke();
}

}*/