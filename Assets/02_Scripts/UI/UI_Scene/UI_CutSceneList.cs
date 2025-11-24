using System.Collections.Generic;
using Apis;
using Default;
using Save.Schema;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UI_CutSceneList : UI_Scene
{
    enum Buttons
    {
        CloseBtn,
    }
    
    public Transform listParent;
    
    public override void Init()
    {
        base.Init();
        Bind<Button>(typeof(Buttons));
        
        GetButton((int)Buttons.CloseBtn).onClick.AddListener(CloseOwn);
    }

    private readonly Dictionary<int, UI_CutSceneSub> subs = new();


    public override void TryActivated(bool force = false)
    {
        foreach (var x in DataAccess.Codex.DataInt(CodexData.CodexType.CutScene))
        {
            if (!CutSceneDatabase.TryGetCutSceneData(x, out var data) || subs.ContainsKey(x)) continue;
            
            UI_CutSceneSub sub = GameManager.UI.MakeSubItem("UI_CutSceneSub", listParent)
                .GetComponent<UI_CutSceneSub>();
            sub.button.onClick.RemoveAllListeners();
            sub.button.onClick.AddListener(() =>
            {
                UI_CutScenePopup popup =
                    GameManager.UI.CreateUI("UI_CutScenePopup", UIType.Popup) as UI_CutScenePopup;

                if (popup != null) popup.Init(data.video);
            });
            sub.nameText.text = data.nameText;
            sub.thumbnail.sprite = ResourceUtil.Load<Sprite>(data.thumbnail);
            subs.Add(x, sub);
        }
        base.TryActivated(force);
    }
}
