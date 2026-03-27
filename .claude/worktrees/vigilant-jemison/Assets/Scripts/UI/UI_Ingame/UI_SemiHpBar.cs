using System;
using Apis;
using Apis.CommonMonster2;
using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UI_SemiHpBar : UI_Ingame
    {

        enum SubItems
        {
            HpBar
        }

        private RectTransform rect;
        private Vector2 offset = new Vector2(0, 200);
        private UI_HpBar hpBar;

        private Actor actor;


        public override void Init()
        {
            base.Init();
            Bind<UI_Base>(typeof(SubItems));


            foreach (SubItems sub in Enum.GetValues(typeof(SubItems)))
            {
                UI_Base item = Get<UI_Base>((int)sub);
                subItems.Add(item);
                item.Init();
            }

            hpBar = Get<UI_Base>((int)SubItems.HpBar).GetComponent<UI_HpBar>();
            rect = Get<UI_Base>((int)SubItems.HpBar).GetComponent<RectTransform>();
        }
        


        public void SetTrans(Transform trans)
        {
            this.targetTrans = trans;
        }

        public void InitActor(Actor actor)
        {
            this.actor = actor;
            hpBar.ResetActors();
            hpBar.Init(actor);
        }

        protected override void PositioningFollower()
        {
            // Debug.Log($"ui pos {(calcPos + offset).x}, {(calcPos + offset).y}");
            rect.anchoredPosition = calcPos + offset;
        }


        protected override void Deactivated()
        {
            base.Deactivated();
        }
    }
}