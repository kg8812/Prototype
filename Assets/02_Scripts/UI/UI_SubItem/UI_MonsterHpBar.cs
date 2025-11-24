using UnityEngine;

namespace UI
{
    public class UI_MonsterHpBar : UI_HpBar
    {
        protected override void InitShields()
        {
            base.InitShields();
            shieldBarWidth = shieldBar.rectTransform.rect.width;
        }

        protected override void UpdateShield(EventParameters _)
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
                boundary.anchoredPosition = shieldBar.rectTransform.anchoredPosition + new Vector2(fillAm * shieldBarWidth, 0);
                boundaryImg.enabled = true;
            }
            else
            {
                boundaryImg.enabled = false;
            }
            
            UpdateText();
        }
    }
}