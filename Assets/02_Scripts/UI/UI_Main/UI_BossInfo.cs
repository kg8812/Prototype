using Default;
using System;
using TMPro;
using UISpaces;
using UnityEngine;

namespace UI
{
    public class UI_BossInfo : UI_Main
    {
        public enum Texts
        {
            Name,
        }
        public enum SubItems
        {
            HpBar,BuffIcons
        }
        TextMeshProUGUI nameText;
        UI_HpBar hpbar;
        UI_BuffCollector buffIcons;
        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<UI_Base>(typeof(SubItems));
            foreach (SubItems sub in Enum.GetValues(typeof(SubItems)))
            {
                UI_Base item = Get<UI_Base>((int)sub);
                subItems.Add(item);
                item.Init();
            }
            nameText = Get<TextMeshProUGUI>((int)Texts.Name);
            hpbar = Get<UI_Base>((int)SubItems.HpBar).GetComponent<UI_HpBar>();
            buffIcons = Get<UI_Base>((int)SubItems.BuffIcons).GetComponent<UI_BuffCollector>();
            buffIcons.SetDescPivot(new Vector2(0, 1));
        }

        public void Init(Actor actor)
        {
            if (!isInit)
                Init();

            hpbar.Init(actor);
            nameText.text = actor.name;
            buffIcons.Init(actor);
        }
    }
}