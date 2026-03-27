using DG.Tweening;
using Default;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UI_HpBar : UI_Base
    {
        readonly float duration = 0.5f;
        [SerializeField] private bool isShowText;

        enum Images
        {
            YellowBar,
            RedBar,
            ShieldBar,
            BoundaryImg
        }

        enum RectTransforms
        {
            Boundary
        }

        enum Texts
        {
            HpText
        }

        
        protected Image yellowBar;
        protected Image redBar;
        protected Image shieldBar;
        protected Image boundaryImg;
        
        protected RectTransform boundary;
        Tweener tween;

        protected float maxHp;
        protected float curHp;
        protected int curShield;

        protected readonly List<Actor> actors = new();
        

        public Transform shieldParent;


        protected float shieldBarWidth;

        private bool hasShield => !ReferenceEquals(shieldBar, null);
        
        public override void Init()
        {
            base.Init();
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(Texts));
            Bind<RectTransform>(typeof(RectTransforms));
            redBar = Get<Image>((int)Images.RedBar);
            yellowBar = Get<Image>((int)Images.YellowBar);
            shieldBar = Get<Image>((int)Images.ShieldBar);
            boundaryImg = Get<Image>((int)Images.BoundaryImg);
            
            redBar.fillAmount = 1;
            yellowBar.fillAmount = 1;
            if (hasShield)
            {
                InitShields();
            }
            
        }

        protected virtual void InitShields()
        {
            shieldBar.fillAmount = 0;
            boundaryImg.enabled = false;
            boundary = Get<RectTransform>((int)RectTransforms.Boundary);
            shieldBarWidth = shieldBar.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        }
        
        public void Init(Actor actor)
        {
            AddActor(actor);

            UpdateRedBar();
            yellowBar.fillAmount = redBar.fillAmount;
            UpdateShield(null);
        }

        public override void TryActivated(bool force = false)
        {
            StopAllCoroutines();
            isUpdating = false;
            redBar.fillAmount = 1;
            yellowBar.fillAmount = 1;
            // Debug.LogError("activated");
            if (hasShield)
            {
                if (shieldBar != null)
                {
                    shieldBar.fillAmount = 0;
                }

                if (boundaryImg != null)
                {
                    boundaryImg.enabled = false;
                }
            }
            base.TryActivated(force);
        }

        float fillAmount;
        bool isUpdating;

        public void AddActor(Actor actor)
        {
            actors.Add(actor);
            actor.AddEvent(EventType.OnHpDown, HpInvoke);
            actor.AddEvent(EventType.OnHpHeal, HealHpBar);
            actor.AddEvent(EventType.OnBarrierChange, UpdateShield);
        }

        public void RemoveActor(Actor actor)
        {
            actors.Remove(actor);
            actor.RemoveEvent(EventType.OnHpDown, HpInvoke);
            actor.RemoveEvent(EventType.OnHpHeal, HealHpBar);
            actor.RemoveEvent(EventType.OnBarrierChange, UpdateShield);
        }

        public void ResetActors()
        {
            actors.ForEach(x =>
            {
                x.RemoveEvent(EventType.OnHpDown, HpInvoke);
                x.RemoveEvent(EventType.OnHpHeal, HealHpBar);
                x.RemoveEvent(EventType.OnBarrierChange, UpdateShield);
            });
            actors.Clear();
        }

        void HpInvoke(EventParameters parameters)
        {
            if (!isUpdating && _activated)
            {
                StartCoroutine(nameof(UpdateYellowBar));
            }

            UpdateRedBar();
        }

        protected override void Deactivated()
        {
            base.Deactivated();
            tween?.Kill();
            ResetActors();
            StopAllCoroutines();
        }

        void UpdateRedBar()
        {
            maxHp = 0;
            curHp = 0;
            actors.ForEach(x => maxHp += x.MaxHp);
            actors.ForEach(x => curHp += x.CurHp);
            redBar.fillAmount = curHp / maxHp;
            // Debug.LogError(curHp / maxHp);
            fillAmount = redBar.fillAmount;
            UpdateText();
        }

        void HealHpBar(EventParameters parameters)
        {
            UpdateRedBar();
            yellowBar.fillAmount = redBar.fillAmount;
            tween?.Kill();
            StopCoroutine(nameof(UpdateYellowBar));
            isUpdating = false;
        }

        IEnumerator UpdateYellowBar()
        {
            tween?.Complete();
            isUpdating = true;
            yield return new WaitForSeconds(duration);
            isUpdating = false;

            tween = DOTween.To(() => yellowBar.fillAmount, x => yellowBar.fillAmount = x, fillAmount, duration - 0.1f);
        }

        protected float fillAm;
        protected int curbarrier = 0;
        protected virtual void UpdateShield(EventParameters _)
        {
            if (shieldBar == null) return;

            curbarrier = 0;

            foreach (var x in actors)
            {
                curbarrier += (int)x.Barrier;
            }

            int hp = Mathf.CeilToInt(maxHp);
            count = curbarrier / hp;
            int value = curbarrier % hp;
            fillAm = value / maxHp;
            shieldBar.fillAmount = fillAm;
            if (fillAm < 0.975f && !Mathf.Approximately(fillAm,0))
            {
                boundary.anchoredPosition = new Vector2(fillAm * shieldBarWidth, 0);
                boundaryImg.enabled = true;
            }
            else
            {
                boundaryImg.enabled = false;
            }
            
            SetShieldIcons();
            UpdateText();
        }

        private Queue<GameObject> icons = new();

        protected int count;

        protected void SetShieldIcons()
        {
            for (int i = count; i < icons.Count && i >= 0; i++)
            {
                GameManager.Factory.Return(icons.Dequeue());
            }
            for (int i = icons.Count; i < count; i++)
            {
                GameObject icon = GameManager.UI.MakeSubItem("ShieldIcon", shieldParent).gameObject;
                icon.transform.SetParent(shieldParent);
                icons.Enqueue(icon);
            }
        }

        protected void UpdateText()
        {
            if (isShowText)
            {
                GetText((int)Texts.HpText).text = $"{curHp}{(curbarrier == 0 ? "" : $"<color=#c3c4c3>(+{curbarrier})</color>")}/{maxHp}";
            }
        }
    }
}