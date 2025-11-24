using System;
using Apis;
using Apis.CommonMonster2;
using Default;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_SemiHpBar : UI_Ingame
    {
        [SerializeField] private bool isShowGroggyBar;
        [SerializeField] private bool isShowMonsterState;

        enum SubItems
        {
            HpBar
        }

        enum GameObjects
        {
            GroggySection
        }

        enum Imgs
        {
            GroggyBarImg
        }


        enum Texts
        {
            MonsterStateText
        }

        private RectTransform rect;
        private Vector2 offset = new Vector2(0, 200);
        private UI_HpBar hpBar;

        private Actor actor;

        private Image groggyBarImg;

        public override void Init()
        {
            base.Init();
            Bind<UI_Base>(typeof(SubItems));
            Bind<GameObject>(typeof(GameObjects));
            Bind<Image>(typeof(Imgs));
            Bind<TextMeshProUGUI>(typeof(Texts));


            foreach (SubItems sub in Enum.GetValues(typeof(SubItems)))
            {
                UI_Base item = Get<UI_Base>((int)sub);
                subItems.Add(item);
                item.Init();
            }

            hpBar = Get<UI_Base>((int)SubItems.HpBar).GetComponent<UI_HpBar>();
            rect = Get<UI_Base>((int)SubItems.HpBar).GetComponent<RectTransform>();
            groggyBarImg = GetImage((int)Imgs.GroggyBarImg);

            Get<GameObject>((int)GameObjects.GroggySection).SetActive(isShowGroggyBar);
            GetText((int)Texts.MonsterStateText).enabled = isShowMonsterState;
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
            // offset = (Vector2)(actor.pivot + actor.topPivot) - new Vector2(0, 50f);
            if (actor is Monster mt)
            {
                if (isShowGroggyBar)
                {
                    mt.CurGroggyGaugeChanged = ChangeGroggyBarImg;
                }

                if (isShowMonsterState && mt is CommonMonster2 cm)
                {
                    cm.AddEvent(EventType.OnStateChanged, ChangeStateText);
                }
            }
        }


        private void ChangeStateText(EventParameters cmParameters)
        {
            GetText((int)Texts.MonsterStateText).text = cmParameters.monsterState.ToString();
        }

        public void ChangeGroggyBarImg(float value)
        {
            groggyBarImg.fillAmount = value;
        }

        protected override void PositioningFollower()
        {
            // Debug.Log($"ui pos {(calcPos + offset).x}, {(calcPos + offset).y}");
            rect.anchoredPosition = calcPos + offset;
        }


        protected override void Deactivated()
        {
            base.Deactivated();

            if (actor is Monster mt)
            {
                if (isShowGroggyBar)
                {
                    mt.CurGroggyGaugeChanged = null;
                }

                if (isShowMonsterState && mt is CommonMonster2 cm)
                {
                    cm.RemoveEvent(EventType.OnStateChanged, ChangeStateText);
                }
            }
        }
    }
}