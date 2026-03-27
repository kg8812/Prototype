using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Apis.UI
{
    public class UI_ActorInfo: UI_Ingame
    {
        enum RectTransforms
        {
            Info
        }

        enum Images
        {
            curHp,
            curGroggy
        }

        enum Texts
        {
            state,
            name
        }
    
        private Vector2 offset = new Vector2(0, 300);

        private RectTransform rect;
        private Vector2 worldPos = Vector2.zero;

        private Image curHp;
        private Image curGroggy;

        public override void Init()
        {
            base.Init();
        
            Bind<RectTransform>(typeof(RectTransforms));
            Bind<Image>(typeof(Images));
            Bind<TextMeshProUGUI>(typeof(Texts));
            
            rect = Get<RectTransform>((int)RectTransforms.Info);
            curHp = Get<Image>((int)Images.curHp);
            curGroggy = Get<Image>((int)Images.curGroggy);
        }

        public void SetInfo(Vector2 targetPos)
        {
            this.targetPos = targetPos;
        }
        public void SetInfo(Transform targetTrans)
        {
            this.targetTrans = targetTrans;
        }


        protected override void PositioningFollower()
        {
            rect.anchoredPosition = calcPos + offset;
        }

        public override void TryActivated(bool force = false)
        {
            SetCurHp(1);
            base.TryActivated(force);
        }


        // ww

        public void SetCurHp(float curHpRatio)
        {
            curHp.fillAmount = curHpRatio;
        }

        public void SetGroggyGauge(float curGroggyRatio)
        {
            curGroggy.fillAmount = curGroggyRatio;
        }

        public void SetName(string name)
        {
            Get<TextMeshProUGUI>((int)Texts.name).text = name;
        }
        
        public void SetState(string state)
        {
            Get<TextMeshProUGUI>((int)Texts.state).text = state;
        }
    }
}