using System.Collections.Generic;
using System.Linq;
using _02_Scripts.UI.UI_SubItem;
using Apis.DataType;
using Apis.Managers;
using Apis.UI;
using Apis.UI.Focus;
using Default;
using Save.Schema;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TipList : UI_Scene
{

    enum Transforms
    {
        Content
    }

    enum GameObjs
    {
        NoneText
    }

    [SerializeField] private List<UIAsset_Toggle> items;
    [SerializeField] private FocusParent focusParent;
    private UI_TipContent _tip;
    private int _tipCnt;

    private Button backButton;
    public override void Init()
    {
        base.Init();
        Bind<Transform>(typeof(Transforms));
        Bind<GameObject>(typeof(GameObjs));
        
        focusParent.InitCheck();
        _tip = GetComponentInChildren<UI_TipContent>();
        subItems.Add(_tip);
        _tip.Init();
    }


    public override void TryActivated(bool force = false)
    {
        Queue<TipDataType> queue = new();
        var list = DataAccess.Codex.DataInt(CodexData.CodexType.Tip)
            .Where(x => DataAccess.Codex.IsOpen(CodexData.CodexType.Tip, x));
        
        foreach (var tip in list)
        {
            if (TipDatabase.TryGetTipData(tip, out var data))
            {
                queue.Enqueue(data);
            }
        }

        _tipCnt = queue.Count;

        for (int i = 0; i < Mathf.Max(items.Count, _tipCnt); i++) 
        {
            if (queue.TryDequeue(out var data))
            {
                if (i >= items.Count)
                {
                    UIAsset_Toggle newToggle = GameManager.UI
                        .MakeSubItem("ListItem", Get<Transform>((int)Transforms.Content))
                        .GetComponent<UIAsset_Toggle>();
                    newToggle.InitCheck();
                    items.Add(newToggle);
                }
                    
                items[i].gameObject.SetActive(true);
                items[i].GetComponentInChildren<TextMeshProUGUI>().text = LanguageManager.LanguageType == LanguageType.English ? data.tipNameEng : data.tipNameKor;
                
                items[i].OnValueChanged.RemoveAllListeners();
                items[i].OnValueChanged.AddListener(isOn =>
                {
                    if(isOn)
                        _tip.SetInfo(data);
                });
            }
            else
            {
                items[i].gameObject.SetActive(false);
            }
        }


        if (_tipCnt == 0)
        {
            Get<GameObject>((int)GameObjs.NoneText).SetActive(true);
            _tip.SetInfo(null);
        }
        else
        {
            Get<GameObject>((int)GameObjs.NoneText).SetActive(false);
            focusParent.Reset(true);
            for (int i = 0; i < _tipCnt; i++)
            {
                focusParent.RegisterElement(items[i]);
            }
            focusParent.FocusReset();
        }
        
        base.TryActivated(force);
    }

    public override void KeyControl()
    {
        base.KeyControl();
        if(_tipCnt != 0)
            focusParent.KeyControl();
    }

    public override void GamePadControl()
    {
        base.GamePadControl();
        if(_tipCnt != 0)
            focusParent.GamePadControl();
    }
}
